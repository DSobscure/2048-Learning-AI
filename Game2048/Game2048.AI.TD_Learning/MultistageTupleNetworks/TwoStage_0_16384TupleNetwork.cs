using Game2048.Game.Library;

namespace Game2048.AI.TD_Learning.MultistageTupleNetworks
{
    public class TwoStage_0_16384TupleNetwork : MultistageTupleNetwork
    {
        private SimpleTupleNetwork stage0TupleNetwork;
        private SimpleTupleNetwork stage16384TupleNetwork;

        public TwoStage_0_16384TupleNetwork(string networkName, int index) : base(networkName, index)
        {
            stage0TupleNetwork = new SimpleTupleNetwork(networkName + "TwoStage_0_16384TupleNetwork_Stage0", index);
            stage16384TupleNetwork = new SimpleTupleNetwork(networkName + "TwoStage_0_16384TupleNetwork_Stage16384", index);
        }

        public override float GetValue(ulong[] symmetricBoards)
        {
            if(BitBoard.MaxTileTest(symmetricBoards[0]) < 16384)
            {
                return stage0TupleNetwork.GetValue(symmetricBoards);
            }
            else
            {
                return stage16384TupleNetwork.GetValue(symmetricBoards);
            }
        }

        public override void Load(out int loadedCount)
        {
            int stage0Count, stage16384Count;
            stage0TupleNetwork.Load(out stage0Count);
            stage16384TupleNetwork.Load(out stage16384Count);
            loadedCount = stage0Count + stage16384Count;
        }

        public override void Save()
        {
            stage0TupleNetwork.Save();
            stage16384TupleNetwork.Save();
        }

        public override void UpdateValue(ulong[] symmetricBoards, float delta)
        {
            if (BitBoard.MaxTileTest(symmetricBoards[0]) < 16384)
            {
                stage0TupleNetwork.UpdateValue(symmetricBoards, delta);
            }
            else
            {
                stage16384TupleNetwork.UpdateValue(symmetricBoards, delta);
            }
        }
    }
}
