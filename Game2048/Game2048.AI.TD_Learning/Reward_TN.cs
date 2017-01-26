using System;

namespace Game2048.AI.TD_Learning
{
    class Reward_TN
    {
        private float learningRate;
        private TupleNetwork tupleNetwork;

        public Reward_TN(string name, float learningRate, int tupleNetworkIndex)
        {
            this.learningRate = learningRate;
            tupleNetwork = new TupleNetwork(name, tupleNetworkIndex);

            int loadedCount;
            tupleNetwork.Load(out loadedCount);
            Console.WriteLine("loaded {0}", loadedCount);
        }
        public void Training(ulong[] symmetricBoards, float desiredValue)
        {
            tupleNetwork.UpdateValue(symmetricBoards, learningRate * (desiredValue - tupleNetwork.GetValue(symmetricBoards)));
        }
        public float Compute(ulong[] symmetricBoards)
        {
            return tupleNetwork.GetValue(symmetricBoards);
        }
        public void SaveClassifier()
        {
            tupleNetwork.Save();
        }
    }
}
