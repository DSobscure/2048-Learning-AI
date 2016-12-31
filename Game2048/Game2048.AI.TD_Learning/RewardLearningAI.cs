using Game2048.Game.Library;
using System;
using System.IO;
using System.Collections.Generic;
using Game2048.AI.NeuralNetwork;
using System.Linq;

namespace Game2048.AI.TD_Learning
{
    public class RewardLearningAI : ILearningAI
    {
        private static float[] ExtractInput(BitBoard board, Direction direction)
        {
            float[] result = new float[17];

            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                int tileNumber = (int)((board.RawBlocks >> shiftBitCount) & 0xf);
                result[shiftBitCount / 4] = tileNumber;
            }
            result[16] = (byte)direction;

            return result;
        }
        private static float ExtractOutput(float[] output)
        {
            float max = output.Max();
            int index = Array.IndexOf(output, max);
            switch(index)
            {
                case 0:
                    return 1;
                case 1:
                    return 4;
                case 2:
                    return 16;
                case 3:
                    return 64;
                case 4:
                    return 256;
                default:
                    return 0;
            }
        }

        private float learningRate;
        private TupleNetwork tupleNetwork;
        private List<TD_State> td_StateChain;
        private MultiLayerPerceptron rewardPerceptron;

        private List<ulong> rawBlocksRecord;

        public RewardLearningAI(float learningRate, float rewardPerceptronLearningRate, Func<float, float> activationFunction, Func<float, float> dActivationFunction, out int loadedCount)
        {
            this.learningRate = learningRate;
            tupleNetwork = new TupleNetwork("RewardLearningTD", 0);
            if (File.Exists("RewardPerceptron"))
            {
                rewardPerceptron = SerializationHelper.Deserialize<MultiLayerPerceptron>(File.ReadAllBytes("RewardPerceptron"));
                rewardPerceptron.LearningRate = rewardPerceptronLearningRate;
                Console.WriteLine($"Load RewardPerceptron, HiddenLayerNumber: {rewardPerceptron.HiddenLayerNumber}, LearningRate: {rewardPerceptron.LearningRate}");
            }
            else
            {
                rewardPerceptron = new MultiLayerPerceptron(17, 5, 2, new int[] { 16, 16 }, rewardPerceptronLearningRate, activationFunction, dActivationFunction);
            }
            td_StateChain = new List<TD_State>();
            rawBlocksRecord = new List<ulong>();

            tupleNetwork.Load(out loadedCount);
        }
        public Game.Library.Game Train(float previousAverageScore = 0, float previousScoreDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            float[] rewardDesiredOutput = new float[5];
            if(game.Score < previousAverageScore - previousScoreDeviation * 2)
            {
                rewardDesiredOutput[0] = 1;
            }
            else if(game.Score < previousAverageScore - previousScoreDeviation)
            {
                rewardDesiredOutput[1] = 1;
            }
            else if(game.Score < previousAverageScore + previousScoreDeviation)
            {
                rewardDesiredOutput[2] = 1;
            }
            else if (game.Score < previousAverageScore + previousScoreDeviation * 2)
            {
                rewardDesiredOutput[3] = 1;
            }
            else
            {
                rewardDesiredOutput[4] = 1;
            }
            UpdateEvaluation(rewardDesiredOutput);
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }
        public Game.Library.Game PlayGame(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = (isUsedRawBoard) ? new Game.Library.Game(rawBoard) : new Game.Library.Game();
            while (!game.IsEnd)
            {
                ulong movedRawBlocks = game.Move(GetBestMove(game.Board, 1));
                ulong blocksAfterAdded = game.Board.RawBlocks;
                TD_State state = new TD_State
                {
                    movedRawBlocks = movedRawBlocks,
                    insertedRawBlocks = blocksAfterAdded
                };
                td_StateChain.Add(state);
                rawBlocksRecord.Add(blocksAfterAdded);
            }
            return game;
        }
        public Direction GetBestMove(BitBoard board, int searchDepth = 1)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;

            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = MutiStepEvaluate(board, direction, searchDepth);
                if (result > maxScore && board.MoveCheck(direction))
                {
                    nextDirection = direction;
                    maxScore = result;
                }
            }
            return nextDirection;
        }
        public float Evaluate(BitBoard board, Direction direction)
        {
            if (board.MoveCheck(direction))
            {
                int result;
                BitBoard boardAfter = board.Move(direction, out result);
                ulong rawBoard = boardAfter.RawBlocks;
                boardAfter.InsertNewTile();
                if (boardAfter.CanMove)
                    return (ExtractOutput(rewardPerceptron.Compute(ExtractInput(board, direction))) + tupleNetwork.GetValue(rawBoard));
                else
                    return -1;
            }
            else
            {
                return 0;
            }
        }
        public float MutiStepEvaluate(BitBoard board, Direction direction, int maxStep)
        {
            float boardValue = Evaluate(board, direction);
            if (maxStep == 1 || boardValue < 0)
            {
                return boardValue;
            }
            else
            {
                int reward;
                BitBoard searchingBoard = board.Move(direction, out reward);
                searchingBoard.InsertNewTile();
                float maxScore = 0;

                bool isFirst = true;
                for (Direction searchingDirection = Direction.Up; searchingDirection <= Direction.Right; searchingDirection++)
                {
                    float result = MutiStepEvaluate(searchingBoard, searchingDirection, maxStep - 1);
                    if (isFirst && searchingBoard.MoveCheck(searchingDirection))
                    {
                        maxScore = result;
                        isFirst = false;
                    }
                    else if (result > maxScore && searchingBoard.MoveCheck(searchingDirection))
                    {
                        maxScore = result;
                    }
                }
                return boardValue + maxScore;
            }
        }
        public void UpdateEvaluation(float[] rewardDesiredOutput)
        {
            BestMoveNode[] bestMoveNodes = new BestMoveNode[td_StateChain.Count];
            for (int i = 0; i < td_StateChain.Count; i++)
            {
                BitBoard board = new BitBoard(td_StateChain[i].insertedRawBlocks);
                Direction nextDirection = GetBestMove(board);
                int nextReward = 0;

                bestMoveNodes[i].bestMove = nextDirection;
                bestMoveNodes[i].rawBlocks = board.RawBlocks;
                bestMoveNodes[i].movedRawBlocks = board.MoveRaw(nextDirection, out nextReward);
                bestMoveNodes[i].reward = nextReward;
                float error;
                rewardPerceptron.Tranning(ExtractInput(board, nextDirection), rewardDesiredOutput, out error);
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                float score = BitBoard.MaxTileTest(bestMoveNodes[i].movedRawBlocks) + tupleNetwork.GetValue(bestMoveNodes[i].movedRawBlocks);
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork.GetValue(td_StateChain[i].movedRawBlocks)));
            }
        }

        public void Save()
        {
            tupleNetwork.Save();
            File.WriteAllBytes("RewardPerceptron", SerializationHelper.Serialize(rewardPerceptron));
        }
    }
}
