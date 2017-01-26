using MsgPack.Serialization;

namespace Game2048.AI.TD_Learning.TupleFeatures
{
    public class FourTupleFeature : TupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        public FourTupleFeature() { }
        public FourTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public FourTupleFeature(int index) : base(4)
        {
            this.index = index;
        }
        public override int GetIndex(ulong blocks)
        {
            switch (index)
            {
                case 1:
                    //ooxx
                    //xoox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xFF00) | ((blocks >> 36) & 0xFF));
                case 2:
                    //xoox
                    //xxoo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xFF00) | ((blocks >> 32) & 0xFF));
                case 3:
                    //xxxx
                    //ooxx
                    //oxxx
                    //oxxx
                    return (int)(((blocks >> 32) & 0xFF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 12) & 0xF));
                case 4:
                    //xxxx
                    //xxox
                    //xooo
                    //xxxx
                    return (int)(((blocks >> 24) & 0xF000) | ((blocks >> 12) & 0xFFF));
                case 5:
                    //xxxx
                    //xxxx
                    //xxox
                    //xooo
                    return (int)(((blocks >> 8) & 0xF000) | (blocks & 0xFFF));
                case 6:
                    //oooo
                    //xxxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xFFFF));
                case 7:
                    //xxxx
                    //oooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 32) & 0xFFFF));
                case 8:
                    //ooxx
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xFF00) | ((blocks >> 40) & 0xFF));
                case 9:
                    //xoox
                    //xoox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xFF00) | ((blocks >> 36) & 0xFF));
                case 10:
                    //xxxx
                    //xoox
                    //xoox
                    //xxxx
                    return (int)(((blocks >> 28) & 0xFF00) | ((blocks >> 20) & 0xFF));
                default:
                    return 0;
            }
        }
    }
}
