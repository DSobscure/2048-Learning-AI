using MsgPack.Serialization;

namespace Game2048.AI.TD_Learning.TupleFeatures
{
    public class SixTupleFeature : TupleFeature
    {
        [MessagePackMember(id: 1, Name = "index")]
        int index;

        public SixTupleFeature() { }
        public SixTupleFeature(float[] tuples, int index) : base(tuples)
        {
            this.index = index;
        }
        public SixTupleFeature(int index) : base(6)
        {
            this.index = index;
        }

        public override int GetIndex(ulong blocks)
        {
            switch (index)
            {
                case 1:
                    //oooo
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 2:
                    //ooox
                    //ooox
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 36) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 36) & 0xF));
                case 3:
                    //oooo
                    //xoxx
                    //xoxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 24) & 0xF));
                case 4:
                    //oooo
                    //oxxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 40) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 28) & 0xF));
                case 5:
                    //ooxx
                    //xoox
                    //xxoo
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 28) & 0xF000) | ((blocks >> 28) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 16) & 0xF));
                case 6:
                    //ooox
                    //xoxx
                    //oxox
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 20) & 0xF));
                case 7:
                    //ooox
                    //xooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 40) & 0xF00000) | ((blocks >> 40) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                case 8:
                    //oooo
                    //oxxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 24) & 0xF00000) | ((blocks >> 24) & 0xF0000) | ((blocks >> 24) & 0xF000) | ((blocks >> 24) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 12) & 0xF));
                case -1:
                    //xxxx
                    //xxxx
                    //xxoo
                    //oooo
                    return (int)(((blocks >> 0) & 0xF00000) | ((blocks >> 0) & 0xF0000) | ((blocks >> 0) & 0xF000) | ((blocks >> 0) & 0xF00) | ((blocks >> 0) & 0xF0) | ((blocks >> 0) & 0xF));
                case -7:
                    //xxxx
                    //xxxx
                    //ooox
                    //xooo
                    return (int)(((blocks >> 8) & 0xF00000) | ((blocks >> 8) & 0xF0000) | ((blocks >> 8) & 0xF000) | ((blocks >> 0) & 0xF00) | ((blocks >> 0) & 0xF0) | ((blocks >> 0) & 0xF));
                case 101:
                    //xxxx
                    //oooo
                    //ooxx
                    //xxxx
                    return (int)(((blocks >> 24) & 0xF00000) | ((blocks >> 24) & 0xF0000) | ((blocks >> 24) & 0xF000) | ((blocks >> 24) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 24) & 0xF));
                case 102:
                    //xxxx
                    //ooox
                    //ooox
                    //xxxx
                    return (int)(((blocks >> 24) & 0xF00000) | ((blocks >> 24) & 0xF0000) | ((blocks >> 24) & 0xF000) | ((blocks >> 20) & 0xF00) | ((blocks >> 20) & 0xF0) | ((blocks >> 20) & 0xF));
                case 202:
                    //xxoo
                    //xxoo
                    //xxoo
                    //xxxx
                    return (int)(((blocks >> 32) & 0xF00000) | ((blocks >> 32) & 0xF0000) | ((blocks >> 24) & 0xF000) | ((blocks >> 24) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 16) & 0xF));
                case 302:
                    //xxxx
                    //xxxx
                    //ooox
                    //ooox
                    return (int)(((blocks >> 8) & 0xF00000) | ((blocks >> 8) & 0xF0000) | ((blocks >> 8) & 0xF000) | ((blocks >> 4) & 0xF00) | ((blocks >> 4) & 0xF0) | ((blocks >> 4) & 0xF));
                case 402:
                    //xooo
                    //xooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 36) & 0xF00000) | ((blocks >> 36) & 0xF0000) | ((blocks >> 36) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                case 502:
                    //xxxx
                    //xooo
                    //xooo
                    //xxxx
                    return (int)(((blocks >> 20) & 0xF00000) | ((blocks >> 20) & 0xF0000) | ((blocks >> 20) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 16) & 0xF));
                case 602:
                    //xxxx
                    //xxxx
                    //xooo
                    //xooo
                    return (int)(((blocks >> 4) & 0xF00000) | ((blocks >> 4) & 0xF0000) | ((blocks >> 4) & 0xF000) | ((blocks >> 0) & 0xF00) | ((blocks >> 0) & 0xF0) | ((blocks >> 0) & 0xF));
                default:
                    return 0;
            }
        }
    }
}
