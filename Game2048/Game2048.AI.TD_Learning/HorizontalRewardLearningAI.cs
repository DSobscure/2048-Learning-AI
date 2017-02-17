using Game2048.Game.Library;
using System;

namespace Game2048.AI.TD_Learning
{
    public class HorizontalRewardLearningAI : LearningAI
    {
        private TupleNetwork tupleNetwork;
        private TupleNetwork rewardTupleNetwork;
        private float rewardLearningRate;

        public HorizontalRewardLearningAI(float learningRate, float rewardLearningRate, TupleNetwork tupleNetwork, TupleNetwork rewardTupleNetwork) : base(learningRate)
        {
            this.rewardLearningRate = rewardLearningRate;
            this.tupleNetwork = tupleNetwork;// new SimpleTupleNetwork("HorizontalRewardValueTN", tupleNetworkIndex);
            this.rewardTupleNetwork = rewardTupleNetwork;// new SimpleTupleNetwork("HorizontalRewardRewardTN", tupleNetworkIndex);
        }
        protected override float Evaluate(BitBoard board, Direction direction)
        {
            int result;
            BitBoard boardAfter = board.Move(direction, out result);
            ulong rawBoard = boardAfter.RawBlocks;
            boardAfter.InsertNewTile();
            var boards = BitBoard.GetSymmetricBoards(rawBoard);
            return 3 + rewardTupleNetwork.GetValue(boards) + tupleNetwork.GetValue(boards);
        }
        public void UpdateEvaluation(float rewardDesiredOutput)
        {
            throw new NotImplementedException();
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
                var boards = BitBoard.GetSymmetricBoards(bestMoveNodes[i].movedRawBlocks);
                bestMoveNodes[i].reward = 3 + rewardTupleNetwork.GetValue(boards);
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
            }
        }

        public override void Save()
        {
            tupleNetwork.Save();
            rewardTupleNetwork.Save();
        }

        public override Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            UpdateEvaluation();
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }

        public override Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage = 0, float previousDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            throw new NotImplementedException();
        }

        protected override unsafe Direction GetBestMove(BitBoard board, int initialStep = 1)
        {
            Direction nextDirection = Direction.No;
            float maxScore = float.MinValue;
            float* scores = stackalloc float[4];
            float totalScore = 0;
            for (Direction direction = Direction.Up; direction <= Direction.Right; direction++)
            {
                float result = ExpectedMaxEvaluate(board, direction, initialStep);
                scores[(int)direction - 1] = result;
                totalScore += result;
                if (result > maxScore && board.MoveCheck(direction))
                {
                    nextDirection = direction;
                    maxScore = result;
                }
            }
            float averageScore = totalScore / 4;
            float scoreDeviation = (float)Math.Sqrt((
                (scores[0] - averageScore) * (scores[0] - averageScore) +
                (scores[1] - averageScore) * (scores[1] - averageScore) +
                (scores[2] - averageScore) * (scores[2] - averageScore) +
                (scores[3] - averageScore) * (scores[3] - averageScore)
                ) / 4);
            if (scoreDeviation <= 0)
            {
                scoreDeviation = float.MaxValue;
            }

            for(int i = 0; i < 4; i++)
            {
                int reward;
                var boards = BitBoard.GetSymmetricBoards(board.MoveRaw((Direction)(i + 1), out reward));
                rewardTupleNetwork.UpdateValue(boards, rewardLearningRate * ((scores[i] - averageScore) / scoreDeviation - rewardTupleNetwork.GetValue(boards)));
            }

            return nextDirection;
        }
    }
}
