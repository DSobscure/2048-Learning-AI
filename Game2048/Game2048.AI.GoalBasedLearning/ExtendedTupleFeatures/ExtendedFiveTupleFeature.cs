using MsgPack.Serialization;

namespace Game2048.AI.GoalBasedLearning.ExtendedTupleFeatures
{
    public class ExtendedFiveTupleFeature : ExtendedTupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        [MessagePackDeserializationConstructor]
        public ExtendedFiveTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public ExtendedFiveTupleFeature(int index) : base(5)
        {
            this.index = index;
        }

        public override int GetIndex(ExtendedBitBoard rawBlocks)
        {
            switch (index)
            {
                case 3:
                    //ooox
                    //xoxx
                    //xoxx
                    //xxxx
                    return (int)(((rawBlocks.upperPart >> (56 - 5*4)) & 0x1F00000) | ((rawBlocks.upperPart >> (48 - 5*3)) & 0xF8000) | ((rawBlocks.upperPart >> (40 - 5*2)) & 0x7C00) | ((rawBlocks.upperPart >> (16 - 5*1)) & 0x3E0) | ((rawBlocks.lowerPart >> 16) & 0x1F));
                default:
                    return 0;
            }
        }
    }
}
