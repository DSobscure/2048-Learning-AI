namespace Game2048.AI.TD_Learning.TupleFeatures
{
    public class FiveTupleFeature : TupleFeature
    {
        int index;

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
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 44) & 0xF0) | ((blocks >> 44) & 0xF));
                case 2:
                    //ooox
                    //ooxx
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 40) & 0xF));
                case 3:
                    //ooox
                    //xoxx
                    //xoxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 36) & 0xF0) | ((blocks >> 24) & 0xF));
                case 4:
                    //ooox
                    //oxxx
                    //oxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 44) & 0xF00) | ((blocks >> 40) & 0xF0) | ((blocks >> 28) & 0xF));
                case 5:
                    //ooxx
                    //xoox
                    //xxox
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 20) & 0xF));
                case 6:
                    //oxox
                    //xoxx
                    //oxox
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 40) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 24) & 0xF0) | ((blocks >> 20) & 0xF));
                case 7:
                    //ooxx
                    //xooo
                    //xxxx
                    //xxxx
                    return (int)(((blocks >> 44) & 0xF0000) | ((blocks >> 44) & 0xF000) | ((blocks >> 32) & 0xF00) | ((blocks >> 32) & 0xF0) | ((blocks >> 32) & 0xF));
                case -3:
                    //xxxo
                    //xooo
                    //xxxo
                    //xxxx
                    return (int)(((blocks >> 32) & 0xF0000) | ((blocks >> 28) & 0xF000) | ((blocks >> 28) & 0xF00) | ((blocks >> 28) & 0xF0) | ((blocks >> 16) & 0xF));
                case 103:
                    //xxxx
                    //oxxx
                    //ooox
                    //oxxx
                    return (int)(((blocks >> 28) & 0xF0000) | ((blocks >> 16) & 0xF000) | ((blocks >> 16) & 0xF00) | ((blocks >> 16) & 0xF0) | ((blocks >> 12) & 0xF));
                default:
                    return 0;
            }
        }
    }
}
