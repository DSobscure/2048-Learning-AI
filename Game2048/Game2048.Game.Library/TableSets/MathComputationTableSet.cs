namespace Game2048.Game.Library.TableSets
{
    public static class MathComputationTableSet
    {
        public static readonly int[] log2Table;

        static MathComputationTableSet()
        {
            log2Table = new int[65536];

            for (int i = 0; i < 16; i++)
            {
                log2Table[1 << i] = i;
            }
        }
    }
}
