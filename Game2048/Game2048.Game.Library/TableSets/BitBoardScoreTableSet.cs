namespace Game2048.Game.Library.TableSets
{
    public static class BitBoardScoreTableSet
    {
        public static readonly int[] tileScoreTable;
        public static readonly float[] rowScoreTable;

        static BitBoardScoreTableSet()
        {
            tileScoreTable = new int[16];
            rowScoreTable = new float[65536];

            tileScoreTable[0] = 0;
            for (int i = 1; i < 16; i++)
            {
                tileScoreTable[i] = 2 << i;
            }

            for (int i = 0; i < 65536; i++)
            {
                for (int shiftBitCount = 0; shiftBitCount < 16; shiftBitCount += 4)
                {
                    int tileNumber = (i >> shiftBitCount) & 0xf;
                    rowScoreTable[i] += tileScoreTable[tileNumber];
                }
            }
        }
    }
}
