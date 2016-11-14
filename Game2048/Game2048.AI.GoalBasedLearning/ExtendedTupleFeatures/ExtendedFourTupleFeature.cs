using MsgPack.Serialization;

namespace Game2048.AI.GoalBasedLearning.ExtendedTupleFeatures
{
    public class ExtendedFourTupleFeature : ExtendedTupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        [MessagePackDeserializationConstructor]
        public ExtendedFourTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public ExtendedFourTupleFeature(int index) : base(4)
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
                    //xxxx
                    //xxxx
                    return (int)(((rawBlocks.upperPart >> (56 - 5*3)) & 0xF8000) | ((rawBlocks.upperPart >> (48 - 5*2)) & 0x7c00) | ((rawBlocks.upperPart >> (40 - 5*1)) & 0x3E0) | ((rawBlocks.upperPart >> 16) & 0x1F));
                case 7:
                    //ooxx
                    //xoox
                    //xxxx
                    //xxxx
                    return (int)(((rawBlocks.upperPart >> (56 - 5 * 3)) & 0xF8000) | ((rawBlocks.upperPart >> (48 - 5 * 2)) & 0x7c00) | ((rawBlocks.upperPart >> (16 - 5 * 1)) & 0x3E0) | ((rawBlocks.upperPart >> 8) & 0x1F));
                default:
                    return 0;
            }
        }
    }
}
