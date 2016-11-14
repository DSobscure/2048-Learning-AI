using MsgPack.Serialization;
using System;

namespace Game2048.AI.GoalBasedLearning
{
    public abstract class ExtendedTupleFeature
    {
        [MessagePackMember(id: 0, Name = "tuples")]
        protected float[] tuples;
        public void UpdateScore(ExtendedBitBoard rawBlocks, float delta)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                tuples[index] += delta;
                if (symmetricIndex != index)
                    tuples[symmetricIndex] += delta;
            }
        }
        public float GetScore(ExtendedBitBoard rawBlocks)
        {
            float sum = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                sum += tuples[index];
                if (symmetricIndex != index)
                    sum += tuples[symmetricIndex];
            }
            return sum;
        }
        public abstract int GetIndex(ExtendedBitBoard rawBlocks);

        public ExtendedBitBoard[] rotateBoards;
        [MessagePackDeserializationConstructor]
        protected ExtendedTupleFeature(float[] tuples)
        {
            this.tuples = tuples;
        }
        protected ExtendedTupleFeature(int tupleNumber)
        {
            tuples = new float[(int)Math.Pow(32, tupleNumber)];
        }

        public void SetSymmetricBoards(ExtendedBitBoard[] rotateSymmetry)
        {
            rotateBoards = rotateSymmetry;
        }
        public unsafe ExtendedBitBoard GetMirrorSymmetricRawBlocks(ExtendedBitBoard rawBlocks)
        {
            uint* reversedRowContents = stackalloc uint[4];

            for (int i = 0; i < 4; i++)
            {
                reversedRowContents[i] = ExtendedBitBoardOperationSet.ReverseRow(ExtendedBitBoardOperationSet.GetRow(rawBlocks, i));
            }

            return ExtendedBitBoardOperationSet.SetRows(reversedRowContents); ;
        }
    }
}
