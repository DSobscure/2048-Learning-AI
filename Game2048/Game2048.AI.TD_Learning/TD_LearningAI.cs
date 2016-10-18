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
        //private TupleNetwork tupleNetwork2;
        private List<TD_State> td_StateChain;
        //private EndgameClassifier endgameClassifier;
        private List<ulong> rawBlocksRecord;


        public TD_LearningAI(float learningRate, out int loadedCount)
        {
            this.learningRate = learningRate;
            tupleNetwork = new TupleNetwork();
            //tupleNetwork2 = new TupleNetwork();
            td_StateChain = new List<TD_State>();
            //endgameClassifier = new EndgameClassifier();
            rawBlocksRecord = new List<ulong>();

            tupleNetwork.Load(out loadedCount);
        }
        public Game.Library.Game Train()
        {
            Game.Library.Game game = PlayGame();
            double errorRate;
            //endgameClassifier.TrainContinuousEndgameBoards(rawBlocksRecord, Math.Max(rawBlocksRecord.Count - 30, 0), out errorRate);
            UpdateEvaluation();
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }
        public Game.Library.Game PlayGame()
        {
            Game.Library.Game game = new Game.Library.Game();
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
                //return result + (endgameClassifier.IsEndgame(boardAfter) ? tupleNetwork2 : tupleNetwork).GetValue(boardAfter);
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
                bestMoveNodes[i].movedRawBlocks = board.MoveRaw(nextDirection, out nextReward);
                bestMoveNodes[i].reward = nextReward;
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(bestMoveNodes[i].movedRawBlocks);
                if (i == td_StateChain.Count - 1)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork.GetValue(td_StateChain[i].movedRawBlocks)));

                //if (endgameClassifier.IsEndgame(bestMoveNodes[i].movedRawBlocks))
                //{
                //    score = bestMoveNodes[i].reward + tupleNetwork2.GetValue(bestMoveNodes[i].movedRawBlocks);
                //    if (i == td_StateChain.Count - 1)
                //    {
                //        score = 0;
                //    }
                //    tupleNetwork2.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork2.GetValue(td_StateChain[i].movedRawBlocks)));
                //}
            }
        }

        public void SaveTupleNetwork()
        {
            tupleNetwork.Save();
        }
    }
}
