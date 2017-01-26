using Game2048.AI.NeuralNetwork;
using Game2048.Game.Library;
using System;
using System.IO;
using System.Linq;

namespace Game2048.AI.TD_Learning
{
    public class MLP_RewardTrained_RewardLearningAI : LearningAI
    {
        private static float[] ExtractInput(ulong movedBoard)
        {
            float[] result = new float[16];

            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                int tileNumber = (int)((movedBoard >> shiftBitCount) & 0xf);
                result[shiftBitCount / 4] = tileNumber;
            }

            return result;
        }
        private static int ExtractOutput(float[] output)
        {
            float max = output.Max();
            int index = Array.IndexOf(output, max);
            switch (index)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                    return 5;
                default:
                    return 0;
            }
        }

        private TupleNetwork tupleNetwork;
        private MultiLayerPerceptron rewardPerceptron;

        public MLP_RewardTrained_RewardLearningAI(float learningRate, float rewardPerceptronLearningRate, Func<float, float> activationFunction, Func<float, float> dActivationFunction, out int loadedCount, int tupleNetworkIndex) : base(learningRate, tupleNetworkIndex)
        {
            tupleNetwork = new TupleNetwork("MLP_RewardTrained_RewardLearningTD", tupleNetworkIndex);
            if (File.Exists("MLP_RewardTrained_RewardPerceptron"))
            {
                rewardPerceptron = SerializationHelper.Deserialize<MultiLayerPerceptron>(File.ReadAllBytes("MLP_RewardTrained_RewardPerceptron"));
                rewardPerceptron.LearningRate = rewardPerceptronLearningRate;
                Console.WriteLine($"Load MLP_RewardTrained_RewardPerceptron, HiddenLayerNumber: {rewardPerceptron.HiddenLayerNumber}, LearningRate: {rewardPerceptron.LearningRate}");
            }
            else
            {
                rewardPerceptron = new MultiLayerPerceptron(16, 5, 1, new int[] { 8 }, rewardPerceptronLearningRate, activationFunction, dActivationFunction);
            }
            tupleNetwork.Load(out loadedCount);
        }
        protected override float Evaluate(BitBoard board, Direction direction)
        {
            if (board.MoveCheck(direction))
            {
                int result;
                BitBoard boardAfter = board.Move(direction, out result);
                ulong rawBoard = boardAfter.RawBlocks;
                boardAfter.InsertNewTile();
                if (boardAfter.CanMove)
                {
                    return (ExtractOutput(rewardPerceptron.Compute(ExtractInput(rawBoard))) + tupleNetwork.GetValue(BitBoard.GetSymmetricBoards(rawBoard)));
                }
                else
                    return -1;
            }
            else
            {
                return 0;
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
                bestMoveNodes[i].reward = ExtractOutput(rewardPerceptron.Compute(ExtractInput(bestMoveNodes[i].movedRawBlocks)));
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                ulong[] symmetricBoards = BitBoard.GetSymmetricBoards(bestMoveNodes[i].movedRawBlocks);
                ulong[] symmetricBoards_td = BitBoard.GetSymmetricBoards(td_StateChain[i].movedRawBlocks);
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(symmetricBoards);
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(symmetricBoards_td, learningRate * (score - tupleNetwork.GetValue(symmetricBoards_td)));
                float error;
                foreach(ulong rawBoard in symmetricBoards_td)
                {
                    rewardPerceptron.Tranning(ExtractInput(rawBoard), rewardDesiredOutput, out error);
                }
            }
        }
        public void UpdateEvaluation()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            tupleNetwork.Save();
            File.WriteAllBytes("MLP_RewardTrained_RewardPerceptron", SerializationHelper.Serialize(rewardPerceptron));
        }

        public override Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            throw new NotImplementedException();
        }

        public override Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage = 0, float previousDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            float[] rewardDesiredOutput = new float[5];
            if (isTrainedByScore)
            {
                if (game.Score < previousAverage - previousDeviation * 2)
                {
                    rewardDesiredOutput[0] = 1;
                }
                else if (game.Score < previousAverage - previousDeviation)
                {
                    rewardDesiredOutput[1] = 1;
                }
                else if (game.Score < previousAverage + previousDeviation)
                {
                    rewardDesiredOutput[2] = 1;
                }
                else if (game.Score < previousAverage + previousDeviation * 2)
                {
                    rewardDesiredOutput[3] = 1;
                }
                else
                {
                    rewardDesiredOutput[4] = 1;
                }
            }
            else
            {
                if (game.Step < previousAverage - previousDeviation * 2)
                {
                    rewardDesiredOutput[0] = 1;
                }
                else if (game.Step < previousAverage - previousDeviation)
                {
                    rewardDesiredOutput[1] = 1;
                }
                else if (game.Step < previousAverage + previousDeviation)
                {
                    rewardDesiredOutput[2] = 1;
                }
                else if (game.Step < previousAverage + previousDeviation * 2)
                {
                    rewardDesiredOutput[3] = 1;
                }
                else
                {
                    rewardDesiredOutput[4] = 1;
                }
            }
            UpdateEvaluation(rewardDesiredOutput);
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }
    }
}
