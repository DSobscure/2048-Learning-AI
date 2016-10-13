using Game2048.Game.Library;
using System.Collections.Generic;

namespace Game2048.AI.TD_Learning
{
    class TD_LearningAI
    {
        private float learningRate;
        private TupleNetwork tupleNetwork;
        private List<TD_State> td_StateChain;

        int winCount;


        public TD_LearningAI(float learningRate)
        {
            this.learningRate = learningRate;
            tupleNetwork = new TupleNetwork();
            td_StateChain = new List<TD_State>();

            //tupleNetwork.Load();
        }
        public Game.Library.Game Train()
        {
            Game.Library.Game game = PlayGame();
            UpdateEvaluation();
            td_StateChain.Clear();

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
                BitBoard nextBoard = board.Move(nextDirection, out nextReward);
                bestMoveNodes[i].bestMove = nextDirection;
                bestMoveNodes[i].reward = nextReward;
                bestMoveNodes[i].movedRawBlocks = nextBoard.RawBlocks;
            }
            bestMoveNodes[td_StateChain.Count - 1].reward = 0;
            for (int i = td_StateChain.Count - 1; i >= 0; i--)
            {
                float score = bestMoveNodes[i].reward + tupleNetwork.GetValue(bestMoveNodes[i].movedRawBlocks);
                tupleNetwork.UpdateValue(td_StateChain[i].movedRawBlocks, learningRate * (score - tupleNetwork.GetValue(td_StateChain[i].movedRawBlocks)));
            }
        }
    }
}
