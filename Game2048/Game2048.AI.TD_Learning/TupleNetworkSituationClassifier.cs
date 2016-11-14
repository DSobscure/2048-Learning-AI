using Game2048.Game.Library;

namespace Game2048.AI.TD_Learning
{
    public class TupleNetworkSituationClassifier
    {
        private float learningRate;
        private TupleNetwork tupleNetwork;

        public TupleNetworkSituationClassifier(string name)
        {
            learningRate = 0.0025f;
            tupleNetwork = new TupleNetwork(name, 1);
            int loadedCount;
            tupleNetwork.Load(out loadedCount);
            System.Console.WriteLine("loaded {0}", loadedCount);
        }
        public void TrainSituation(ulong rawBlocks, bool isInSituation)
        {
            rawBlocks = BitBoard.InversBlocks(rawBlocks);
            int score = (isInSituation) ? 100 : -100;
            tupleNetwork.UpdateValue(rawBlocks, learningRate * (score - tupleNetwork.GetValue(rawBlocks)));
        }
        public bool IsInSituation(ulong rawBlocks)
        {
            rawBlocks = BitBoard.InversBlocks(rawBlocks);
            return tupleNetwork.GetValue(rawBlocks) > 0;
        }
        public void SaveClassifier()
        {
            tupleNetwork.Save();
        }
    }
}
