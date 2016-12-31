using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game2048.Game.Library;

namespace Game2048.AI.TD_Learning
{
    public class StepEvaluationLearningAI : ILearningAI
    {
        private float learningRate;
        private TupleNetwork tupleNetwork;
        private List<TD_State> td_StateChain;

        private List<ulong> rawBlocksRecord;

        public StepEvaluationLearningAI(float learningRate, out int loadedCount)
        {
            this.learningRate = learningRate;
            tupleNetwork = new TupleNetwork("StepEvaluationTN", 0);
            td_StateChain = new List<TD_State>();
            rawBlocksRecord = new List<ulong>();

            tupleNetwork.Load(out loadedCount);
        }

        public Game.Library.Game PlayGame(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = (isUsedRawBoard) ? new Game.Library.Game(rawBoard) : new Game.Library.Game();
            while (!game.IsEnd)
            {
                ulong movedRawBlocks = game.Move(GetBestMove(game.Board, 1, game.Step));
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

        public void Save()
        {
            tupleNetwork.Save();
        }

        public Game.Library.Game Train(float previousAverageScore = 0, float previousScoreDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            UpdateEvaluation();
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }

        public Direction GetBestMove(BitBoard board, int searchDepth, int currentStep)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;

            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = MutiStepEvaluate(board, direction, searchDepth, currentStep);
                if (result > maxScore && board.MoveCheck(direction))
                {
                    nextDirection = direction;
                    maxScore = result;
                }
            }
            return nextDirection;
        }
        public float Evaluate(BitBoard board, Direction direction, int currentStep)
        {
            if (board.MoveCheck(direction))
            {
                int result;
                BitBoard boardAfter = board.Move(direction, out result);
                ulong rawBoard = boardAfter.RawBlocks;
                boardAfter.InsertNewTile();
                if (boardAfter.CanMove)
                    return 1 + tupleNetwork.GetValue(rawBoard);
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
        public float MutiStepEvaluate(BitBoard board, Direction direction, int searchDepth, int currentStep)
        {
            float boardValue = Evaluate(board, direction, currentStep);
            if (searchDepth == 1 || boardValue < 0)
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
                    float result = MutiStepEvaluate(searchingBoard, searchingDirection, searchDepth - 1, currentStep + 1);
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
                Direction nextDirection = GetBestMove(board, 1, i);
                int nextReward = 0;

                bestMoveNodes[i].bestMove = nextDirection;
                bestMoveNodes[i].rawBlocks = board.RawBlocks;
                bestMoveNodes[i].movedRawBlocks = board.MoveRaw(nextDirection, out nextReward);
                bestMoveNodes[i].reward = nextReward;
            }
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                float score = 1 + tupleNetwork.GetValue(bestMoveNodes[i].movedRawBlocks);
                if (i == td_StateChain.Count - 1 && bestMoveNodes[i].rawBlocks == bestMoveNodes[i].movedRawBlocks)
                {
                    score = 0;
                }
                tupleNetwork.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork.GetValue(td_StateChain[i].movedRawBlocks)));
            }
        }
    }
}
