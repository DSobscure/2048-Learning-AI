using Game2048.AI.NeuralNetwork;
using Game2048.Game.Library;
using System.Collections.Generic;
using System;

namespace Game2048.AI.TD_Learning
{
    class TD_LearningAI
    {
        private float learningRate;
        private TupleNetwork tupleNetwork;
        private TupleNetwork normalTupleNetwork;
        private TupleNetwork endgameTupleNetwork;

        private List<TD_State> td_StateChain;

        private EndgameClassifier endgameClassifier;
        private EndgameClassifier endgameClassifier2;

        private List<ulong> rawBlocksRecord;
        private bool isUsedEndgameNetwork;


        public TD_LearningAI(float learningRate, out int loadedCount)
        {
            this.learningRate = learningRate;
            //tupleNetwork = new TupleNetwork("TrainedEndgameTD");
            normalTupleNetwork = new TupleNetwork("TrainedNormalTD", 1);
            endgameTupleNetwork = new TupleNetwork("TrainedEndgameTD", 1);
            td_StateChain = new List<TD_State>();
            endgameClassifier = new EndgameClassifier("EndgameClassifier_LearningRate0.01_Layers64_64_64_16_4");
            rawBlocksRecord = new List<ulong>();

            //tupleNetwork.Load(out loadedCount);
            int normalLoadedCount, endgameLoadedCount;
            normalTupleNetwork.Load(out normalLoadedCount);
            endgameTupleNetwork.Load(out endgameLoadedCount);
            loadedCount = normalLoadedCount + endgameLoadedCount;

            
        }
        public Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            //for (int i = changeTupleIndex; i + 2000 < rawBlocksRecord.Count; i++)
            //{
            //    NormalRawBoardSet.AddRawBoard(rawBlocksRecord[i]);
            //}
            //if (rawBlocksRecord.Count > 200)
            //    EndgameRawBoardSet.AddRawBoard(rawBlocksRecord[rawBlocksRecord.Count - 20]);
            //UpdateEvaluation();
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }
        public Game.Library.Game PlayGame(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            tupleNetwork = normalTupleNetwork;
            isUsedEndgameNetwork = false;

            Game.Library.Game game = (isUsedRawBoard) ? new Game.Library.Game(rawBoard) : new Game.Library.Game();
            while (!game.IsEnd)
            {
                ulong movedRawBlocks = game.Move(GetBestMove(game.Board));
                ulong blocksAfterAdded = game.Board.RawBlocks;
                TD_State state = new TD_State
                {
                    movedRawBlocks = movedRawBlocks,
                    insertedRawBlocks = blocksAfterAdded
                };
                td_StateChain.Add(state);
                rawBlocksRecord.Add(blocksAfterAdded);
                //if (!isUsedEndgameNetwork && endgameClassifier.IsEndgame(blocksAfterAdded))
                //{
                //    isUsedEndgameNetwork = true;
                //    tupleNetwork = endgameTupleNetwork;
                //    //changeTupleIndex = game.Step - 1;
                //}
                //else if(isUsedEndgameNetwork && !isUsedEndgame2Network && endgameClassifier2.IsEndgame(blocksAfterAdded))
                //{
                //    isUsedEndgame2Network = true;
                //    tupleNetwork = endgame2TupleNetwork;
                //}

            }
            return game;
        }
        public Direction GetBestMove(BitBoard board)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;

            bool isFirst = true;
            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = Evaluate(board, direction);
                if (isFirst && board.MoveCheck(direction))
                {
                    nextDirection = direction;
                    maxScore = result;
                    isFirst = false;
                }
                else if (result > maxScore && board.MoveCheck(direction))
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
                ulong boardAfter = board.MoveRaw(direction, out result);
                return result + tupleNetwork.GetValue(boardAfter);
            }
            else
            {
                return 0;
            }
        }
        public void UpdateEvaluation()
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
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(bestMoveNodes[i].movedRawBlocks);
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork.GetValue(td_StateChain[i].movedRawBlocks)));
            }
        }

        public void SaveTupleNetwork()
        {
            //tupleNetwork.Save();
            //normalTupleNetwork.Save();
            //endgameTupleNetwork.Save();
        }
    }
}
