using MsgPack.Serialization;

namespace Game2048.AI.TD_Learning.TupleFeatures
{
    public class FourTupleFeature : TupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        [MessagePackDeserializationConstructor]
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
                    //oooo
                    //xxxx
                    //xxxx
                    //xxxx
                    return (int)((blocks >> 48) & 0xFFFF);
                case 2:
                    //xxxx
                    //oooo
                    //xxxx
                    //xxxx
                    return (int)((blocks >> 32) & 0xFFFF);
                case 3:
                    //oxxx
                    //oxxx
                    //oxxx
                    //oxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 12) & 0xF));
                case 4:
                    //xoxx
                    //xoxx
                    //xoxx
                    //xoxx
                    return (int)(((blocks >> 44) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 20) & 0xF0) | ((blocks >> 8) & 0xF));
                case 5:
                    //ooxx
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xFF00) | ((blocks >> 40) & 0xFF));
                case 6:
                    //xoox
                    //xoox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xFF00) | ((blocks >> 36) & 0xFF));
                case 7:
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
