using Game2048.Game.Library;
using System;

namespace Game2048.AI.TD_Learning
{
    public class VerticalRewardLearningAI : LearningAI
    {
        private TupleNetwork tupleNetwork;
        private TupleNetwork rewardTupleNetwork;
        private float rewardLearningRate;

        public VerticalRewardLearningAI(float learningRate, float rewardLearningRate, TupleNetwork tupleNetwork, TupleNetwork rewardTupleNetwork) : base(learningRate)
        {
            this.rewardLearningRate = rewardLearningRate;
            this.tupleNetwork = tupleNetwork;
            this.rewardTupleNetwork = rewardTupleNetwork;
        }
        protected override float Evaluate(BitBoard board, Direction direction)
        {
            int result;
            BitBoard boardAfter = board.Move(direction, out result);
            ulong rawBoard = boardAfter.RawBlocks;

            var boards = BitBoard.GetSymmetricBoards(rawBoard);
            return 3 + rewardTupleNetwork.GetValue(boards) + tupleNetwork.GetValue(boards);
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
                rewardTupleNetwork.UpdateValue(symmetricBoards_td, rewardLearningRate * (rewardDesiredOutput - rewardTupleNetwork.GetValue(symmetricBoards_td)));
            }
        }
        public void UpdateEvaluation()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            tupleNetwork.Save();
            rewardTupleNetwork.Save();
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
    }
}
