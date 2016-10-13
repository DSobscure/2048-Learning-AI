using Game2048.Game.Library.TableSets;

namespace Game2048.Game.Library
{
    public static class BitBoardOperationSet
    {
        public static ushort ReverseRow(int rowContent)
        {
            return (ushort)((rowContent >> 12) | ((rowContent >> 4) & 0x00F0) | ((rowContent << 4) & 0x0F00) | (rowContent << 12));
        }
        public static ulong UnpackColumn(int columnContent)
        {
            ulong ulongColumnContent = (ulong)columnContent;
            return (ulongColumnContent | (ulongColumnContent << 12) | (ulongColumnContent << 24) | (ulongColumnContent << 36)) & 0x000F000F000F000F;
        }
        public static ulong Transpose(ulong board)
        {
            ulong result;
            ulong diagonal4x4block = board & 0xFF00FF0000FF00FF;
            ulong topRight4x4block = board & 0x00FF00FF00000000; ;
            ulong downLeft4x4block = board & 0x00000000FF00FF00;

            ulong swaped = diagonal4x4block | (topRight4x4block >> 24) | (downLeft4x4block << 24);

            ulong diagonalNet = swaped & 0xF0F00F0FF0F00F0F;
            ulong upperSparse4corner = swaped & 0x0F0F00000F0F0000; ;
            ulong lowerSparse4corner = swaped & 0x0000F0F00000F0F0;

            result = diagonalNet | (upperSparse4corner >> 12) | (lowerSparse4corner << 12);
            return result;
        }
        public static float GetScore(ulong board)
        {
            float score = 0;
            for (int i = 0; i < 4; i++)
            {
                score += BitBoardScoreTableSet.rowScoreTable[GetRow(board, i)];
            }
            return score;
        }
        public static ushort GetRow(ulong board, int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return (ushort)((board >> 48) & 0xFFFF);
                case 1:
                    return (ushort)((board >> 32) & 0xFFFF);
                case 2:
                    return (ushort)((board >> 16) & 0xFFFF);
                case 3:
                    return (ushort)((board) & 0xFFFF);
            }
            return (ushort)((board) & 0xFFFF);
        }
        public static ushort GetColumn(ulong board, int columnIndex)
        {
            ulong columnMask = (0xf000f000f000f000 >> (columnIndex * 4));

            board = (board & columnMask) << 4 * columnIndex;
            return (ushort)(GetRow(board, 0) | (GetRow(board, 1) >> 4) | (GetRow(board, 2) >> 8) | (GetRow(board, 3) >> 12));
        }
        public static ulong SetRows(ushort[] rows)
        {
            ulong result = 0;
            result |= (ulong)rows[0] << 48;
            result |= (ulong)rows[1] << 32;
            result |= (ulong)rows[2] << 16;
            result |= rows[3];
            return result;
        }
        public static ulong SetColumns(ushort[] columns)
        {
            return Transpose(SetRows(columns));
        }
    }
}
