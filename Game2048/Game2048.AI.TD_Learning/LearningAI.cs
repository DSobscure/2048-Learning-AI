using Game2048.Game.Library;
using System.Collections.Generic;

namespace Game2048.AI.TD_Learning
{
    public abstract class LearningAI
    {
        protected float learningRate;
        protected List<TD_State> td_StateChain;
        protected List<ulong> rawBlocksRecord;

        public LearningAI(float learningRate)
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

        protected virtual Direction GetBestMove(BitBoard board, int depth = 1)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;

            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = ExpectedMaxEvaluate(board, direction, depth);
                if (result > maxScore && board.MoveCheck(direction))
                {
                    nextDirection = direction;
                    maxScore = result;
                }
            }
            return nextDirection;
        }
        protected abstract float Evaluate(BitBoard board, Direction direction);
        public float ExpectedMaxEvaluate(BitBoard board, Direction direction, int depth)
        {
            if (depth <= 1)
            {
                return Evaluate(board, direction);
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
