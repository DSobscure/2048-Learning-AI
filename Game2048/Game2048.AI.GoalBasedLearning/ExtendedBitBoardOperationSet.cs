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
            return (ushort)((rowContent >> 24) | ((rowContent >> 8) & 0x0000FF00) | ((rowContent << 8) & 0x00FF0000) | (rowContent << 24));
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
        public unsafe static ExtendedBitBoard SetColumns(uint* columns)
        {
            ExtendedBitBoard result = new ExtendedBitBoard { lowerPart = 0, upperPart = 0 };
            throw new System.NotImplementedException();
            return result;
        }
    }
}
