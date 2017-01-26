using Game2048.Game.Library;
using System;
using System.IO;
using System.Linq;

namespace Game2048.AI.TD_Learning
{
    public class TN_RewardTrained_RewardLearningAI : LearningAI
    {
        private TupleNetwork tupleNetwork;
        private Reward_TN rewardTN;

        public TN_RewardTrained_RewardLearningAI(float learningRate, float rewardLearningRate, out int loadedCount, int tupleNetworkIndex) : base(learningRate, tupleNetworkIndex)
        {
            tupleNetwork = new TupleNetwork("TN_RewardTrained_RewardLearningTD", tupleNetworkIndex);
            rewardTN = new Reward_TN("RewardTrained_RewardTN", rewardLearningRate, tupleNetworkIndex);
            tupleNetwork.Load(out loadedCount);
        }
        protected override float Evaluate(BitBoard board, Direction direction)
        {
            if (board.MoveCheck(direction))
            {
                int result;
                BitBoard boardAfter = board.Move(direction, out result);
                ulong rawBoard = boardAfter.RawBlocks;
                boardAfter.InsertNewTile();
                if (boardAfter.CanMove)
                {
                    var boards = BitBoard.GetSymmetricBoards(rawBoard);
                    return (rewardTN.Compute(boards) + tupleNetwork.GetValue(boards));
                }
                else
                    return -1;
            }
            else
            {
                return 0;
            }
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
                bestMoveNodes[i].reward = rewardTN.Compute(boards);
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
                rewardTN.Training(symmetricBoards_td, rewardDesiredOutput);
            }
        }
        public void UpdateEvaluation()
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            tupleNetwork.Save();
            rewardTN.SaveClassifier();
        }

        public override Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            throw new NotImplementedException();
        }

        public override Game.Library.Game RewardTrain(bool isTrainedByScore, float previousAverage = 0, float previousDeviation = 0, bool isUsedRawBoard = false, ulong rawBoard = 0)
        {
            Game.Library.Game game = PlayGame(isUsedRawBoard, rawBoard);
            float rewardDesiredOutput = 3;
            float delta = 0;
            if (isTrainedByScore)
            {
                delta = (game.Score - previousAverage) / previousDeviation;
                
            }
            else
            {
                delta = (game.Step - previousAverage) / previousDeviation;
            }
            rewardDesiredOutput = (float)(3 + Math.Sign(delta) * (Math.Pow(3, Math.Abs(delta)) - 1));
            //rewardDesiredOutput = (float)(3 + delta);
            //rewardDesiredOutput = (float)(3 + 3 * delta);

            UpdateEvaluation(rewardDesiredOutput);
            td_StateChain.Clear();
            rawBlocksRecord.Clear();

            return game;
        }
    }
}
