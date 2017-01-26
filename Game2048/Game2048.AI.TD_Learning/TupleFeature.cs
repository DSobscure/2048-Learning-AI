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
            tuples[GetIndex(rawBlocks)] += delta;
        }
        public float GetScore(ulong blocks)
        {
            return tuples[GetIndex(blocks)];
        }
        public abstract int GetIndex(ulong blocks);

        public TupleFeature() { }
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
