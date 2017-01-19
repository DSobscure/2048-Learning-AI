using MsgPack.Serialization;
using System;

namespace Game2048.AI.TD_Learning
{
    public abstract class TupleFeature
    {
        [MessagePackMember(id:0, Name = "tuples")]
        protected float[] tuples;
        public void UpdateScore(ulong rawBlocks, float delta)
        {
            int index = GetIndex(rawBlocks);
            tuples[index] += delta;
        }
        public float GetScore(ulong blocks)
        {
            int index = GetIndex(blocks);
            return tuples[index];
        }
        public abstract int GetIndex(ulong blocks);

        [MessagePackDeserializationConstructor]
        protected TupleFeature(float[] tuples)
        {
            this.tuples = tuples;
        }
        protected TupleFeature(int tupleNumber)
        {
            tuples = new float[(int)Math.Pow(16, tupleNumber)];
        }
    }
}
