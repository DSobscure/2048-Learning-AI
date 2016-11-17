namespace Game2048.AI.GoalBasedLearning
{
    public static class ExtendedBitBoardOperationSet
    {
        public static uint GetRow(ExtendedBitBoard rawBlocks, int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return (uint)((rawBlocks.upperPart >> 32) & 0xFFFFFFFF);
                case 1:
                    return (uint)(rawBlocks.upperPart & 0xFFFFFFFF);
                case 2:
                    return (uint)((rawBlocks.lowerPart >> 32) & 0xFFFFFFFF);
                case 3:
                    return (uint)(rawBlocks.lowerPart & 0xFFFFFFFF);
            }
            return 0;
        }
        public static uint ReverseRow(uint rowContent)
        {
            return ((rowContent >> 24) | ((rowContent >> 8) & 0x0000FF00) | ((rowContent << 8) & 0x00FF0000) | (rowContent << 24));
        }
        public unsafe static ExtendedBitBoard SetRows(uint* rows)
        {
            ExtendedBitBoard result = new ExtendedBitBoard { lowerPart = 0, upperPart = 0 };
            result.upperPart |= (ulong)rows[0] << 32;
            result.upperPart |= rows[1];
            result.lowerPart |= (ulong)rows[2] << 32;
            result.lowerPart |= rows[3];
            return result;
        }
        public static ExtendedBitBoard Transpose(ExtendedBitBoard board)
        {
            ExtendedBitBoard result = new ExtendedBitBoard { upperPart = 0, lowerPart = 0 };
            ExtendedBitBoard diagonal4x4block = board & new ExtendedBitBoard { upperPart = 0xFFFF0000FFFF0000, lowerPart = 0x0000FFFF0000FFFF };
            ExtendedBitBoard topRight4x4block = board & new ExtendedBitBoard { upperPart = 0x0000FFFF0000FFFF, lowerPart = 0 };
            ExtendedBitBoard downLeft4x4block = board & new ExtendedBitBoard { upperPart = 0, lowerPart = 0xFFFF0000FFFF0000 };

            ExtendedBitBoard swaped = diagonal4x4block | (topRight4x4block >> 48) | (downLeft4x4block << 48);

            ExtendedBitBoard diagonalNet = swaped & new ExtendedBitBoard { upperPart = 0xFF00FF0000FF00FF, lowerPart = 0xFF00FF0000FF00FF };
            ExtendedBitBoard upperSparse4corner = swaped & new ExtendedBitBoard { upperPart = 0x00FF00FF00000000, lowerPart = 0x00FF00FF00000000 };
            ExtendedBitBoard lowerSparse4corner = swaped & new ExtendedBitBoard { upperPart = 0x00000000FF00FF00, lowerPart = 0x00000000FF00FF00 };

            result = diagonalNet | (upperSparse4corner >> 24) | (lowerSparse4corner << 24);
            return result;
        }
        public unsafe static ExtendedBitBoard SetColumns(uint* columns)
        {
            return Transpose(SetRows(columns));
        }
    }
}
