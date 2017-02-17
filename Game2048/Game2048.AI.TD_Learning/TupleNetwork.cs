namespace Game2048.AI.TD_Learning
{
    public abstract class TupleNetwork
    {
        public string NetworkName { get; private set; }
        protected TupleNetwork(string networkName, int index)
        {
            NetworkName = networkName;
        }
        public abstract float GetValue(ulong[] symmetricBoards);
        public abstract void UpdateValue(ulong[] symmetricBoards, float delta);
        public abstract void Save();
        public abstract void Load(out int loadedCount);
    }
}
