using Game2048.AI.TD_Learning;
using Game2048.Game.Library;
using System.Collections.Generic;

namespace Game2048.AI.GoalBasedLearning
{
    public class GoalBasedTD_LearningAI : ILearningAI
    {
        private float learningRate;
        private ExtendedTupleNetwork tupleNetwork;
        private List<TD_State> td_StateChain;

        private List<ulong> rawBlocksRecord;

        public GoalBasedTD_LearningAI(float learningRate, out int loadedCount)
        {
            this.learningRate = learningRate;
            tupleNetwork = new ExtendedTupleNetwork("SuperNormalGoalBasedTD", 1);
            td_StateChain = new List<TD_State>();
            rawBlocksRecord = new List<ulong>();

            tupleNetwork.Load(out loadedCount);
        }
        public Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            UpdateEvaluation();
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
        public Direction GetBestMove(BitBoard board, int initialStep = 1)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;

            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = MutiStepEvaluate(board, direction, initialStep);
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
                    return (result + tupleNetwork.GetValue(BoardDifferenceComputer.Compute(BestBoardGenerator.BasicLayeredTile(rawBoard), rawBoard)));
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
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(BoardDifferenceComputer.Compute(BestBoardGenerator.BasicLayeredTile(bestMoveNodes[i].movedRawBlocks), bestMoveNodes[i].movedRawBlocks));
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(BoardDifferenceComputer.Compute(BestBoardGenerator.BasicLayeredTile(td_StateChain[i].movedRawBlocks), td_StateChain[i].movedRawBlocks), learningRate * (score - tupleNetwork.GetValue(BoardDifferenceComputer.Compute(BestBoardGenerator.BasicLayeredTile(td_StateChain[i].movedRawBlocks), td_StateChain[i].movedRawBlocks))));
            }
        }

        public void Save()
        {
            tupleNetwork.Save();
        }
    }
}
