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
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 48) & 0xF0) | ((blocks >> 48) & 0xF));
                case 2:
                    //ooox
                    //oxxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 48) & 0xF0) | ((blocks >> 44) & 0xF));
                case 3:
                    //ooox
                    //xoxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 48) & 0xF0) | ((blocks >> 40) & 0xF));
                case 4:
                    //ooxx
                    //ooxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 5:
                    //ooxx
                    //xxoo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                case 6:
                    //oxox
                    //xoxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 28) & 0xF));
                case 7:
                    //ooxx
                    //xoox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 48) & 0xF000) | ((blocks >> 48) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 36) & 0xF));
                case 8:
                    //xxxx
                    //ooox
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 32) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 28) & 0xF));
                case 9:
                    //xooo
                    //xoxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 44) & 0xF0) | ((blocks >> 40) & 0xF));
                case -3:
                    //xxxx
                    //oxxx
                    //ooxx
                    //oxxx
                    return (int)(((blocks >> 32) & 0xF000) | ((blocks >> 20) & 0xF00) | ((blocks >> 20) & 0xF0) | ((blocks >> 12) & 0xF));
                case -4:
                    //xxoo
                    //xxoo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                default:
                    return 0;
            }
        }
    }
}
