using MsgPack.Serialization;

namespace Game2048.AI.TD_Learning.TupleFeatures
{
    public class FiveTupleFeature : TupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        public FiveTupleFeature() { }
        public FiveTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public FiveTupleFeature(int index) : base(5)
        {
            this.index = index;
        }

        public override int GetIndex(ulong blocks)
        {
            switch (index)
            {
                case 1:
                    //oooo
                    //oxxx
                    //xxxx
                    //xxxx
                    return (int)((blocks >> 44) & 0xFFFFF);
                case 2:
                    //xxxx
                    //oooo
                    //oxxx
                    //xxxx
                    return (int)((blocks >> 28) & 0xFFFFF);
                case 3:
                    //xxxx
                    //xxxx
                    //oooo
                    //oxxx
                    return (int)((blocks >> 12) & 0xFFFFF);
                case 4:
                    //xxoo
                    //xooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 36) & 0xFF000) | ((blocks >> 32) & 0xFFF));
                case 5:
                    //xxxx
                    //xxoo
                    //xooo
                    //xxxx
                    return (int)(((blocks >> 20) & 0xFF000) | ((blocks >> 16) & 0xFFF));
                case 6:
                    //xxxx
                    //xxxx
                    //xxoo
                    //xooo
                    return (int)(((blocks >> 4) & 0xFF000) | ((blocks) & 0xFFF));
                default:
                    return 0;
            }
        }
    }
}
