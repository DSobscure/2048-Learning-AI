using Game2048.Game.Library;
using System;

namespace Game2048.AI.TD_Learning
{
    public class CrossRewardLearningAI : LearningAI
    {
        private TupleNetwork tupleNetwork;
        private TupleNetwork verticalRewardTN;
        private TupleNetwork horizontalRewardTN;
        private float rewardLearningRate;

        public CrossRewardLearningAI(float learningRate, float rewardLearningRate, TupleNetwork tupleNetwork, TupleNetwork verticalRewardTN, TupleNetwork horizontalRewardTN) : base(learningRate)
        {
            this.rewardLearningRate = rewardLearningRate;
            this.tupleNetwork = tupleNetwork;
            this.verticalRewardTN = verticalRewardTN;
            this.horizontalRewardTN = horizontalRewardTN;
        }
        protected override float Evaluate(BitBoard board, Direction direction)
        {
            int result;
            BitBoard boardAfter = board.Move(direction, out result);
            ulong rawBoard = boardAfter.RawBlocks;
            boardAfter.InsertNewTile();
            var boards = BitBoard.GetSymmetricBoards(rawBoard);
            return 3 + verticalRewardTN.GetValue(boards) + horizontalRewardTN.GetValue(boards) + tupleNetwork.GetValue(boards);
        }
        public void UpdateEvaluation(float rewardDesiredOutput)
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
                bestMoveNodes[i].reward = 3 + verticalRewardTN.GetValue(boards) + horizontalRewardTN.GetValue(boards);
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
                verticalRewardTN.UpdateValue(symmetricBoards_td, rewardLearningRate * (rewardDesiredOutput - verticalRewardTN.GetValue(symmetricBoards_td)));
            }
        }
        public void UpdateEvaluation()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            tupleNetwork.Save();
            verticalRewardTN.Save();
            horizontalRewardTN.Save();
        }

        public override Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            throw new NotImplementedException();
        }

        public override Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage = 0, float previousDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            float delta = 0;
            if (isTrainedByScore)
            {
                delta = (game.Score - previousAverage) / previousDeviation;

            }
            else
            {
                delta = (game.Step - previousAverage) / previousDeviation;
            }
            UpdateEvaluation(delta);
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
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
            
            for (int i = 0; i < 4; i++)
            {
                int reward;
                var boards = BitBoard.GetSymmetricBoards(board.MoveRaw((Direction)(i + 1), out reward));
                horizontalRewardTN.UpdateValue(boards, rewardLearningRate * ((scores[i] - averageScore) / scoreDeviation - horizontalRewardTN.GetValue(boards)));
            }

            return nextDirection;
        }
    }
}
