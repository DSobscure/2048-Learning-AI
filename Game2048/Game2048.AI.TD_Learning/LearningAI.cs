using Game2048.Game.Library;
using System.Collections.Generic;

namespace Game2048.AI.TD_Learning
{
    public abstract class LearningAI
    {
        protected float learningRate;
        protected List<TD_State> td_StateChain;
        protected List<ulong> rawBlocksRecord;

        public LearningAI(float learningRate, int tupleNetworkIndex)
        {
            this.learningRate = learningRate;
            td_StateChain = new List<TD_State>();
            rawBlocksRecord = new List<ulong>();
        }

        public abstract Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0);
        public abstract Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage, float previousDeviation, bool isUsedRawBoard = false, ulong rawBoard = 0);
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
        public abstract void Save();

        protected Direction GetBestMove(BitBoard board, int initialStep = 1)
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
        protected abstract float Evaluate(BitBoard board, Direction direction);
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
    }
}
