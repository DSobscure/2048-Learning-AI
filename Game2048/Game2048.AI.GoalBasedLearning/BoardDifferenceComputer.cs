namespace Game2048.AI.GoalBasedLearning
{
    public static class BoardDifferenceComputer
    {
        public static ExtendedBitBoard ToExtendedBitBoard(ulong rawBlocks)
        {
            ExtendedBitBoard result = new ExtendedBitBoard { lowerPart = 0, upperPart = 0 };
            for (int shiftBitCount = 0; shiftBitCount < 32; shiftBitCount += 4)
            {
                byte tile = (byte)((rawBlocks >> shiftBitCount) & 0xf);
                result.lowerPart |= ((ulong)tile << (shiftBitCount * 2));
            }
            for (int shiftBitCount = 32; shiftBitCount < 64; shiftBitCount += 4)
            {
                byte tile = (byte)((rawBlocks >> shiftBitCount) & 0xf);
                result.upperPart |= ((ulong)tile << ((shiftBitCount - 32) * 2));
            }
            return result;
        }
        public static ExtendedBitBoard Compute(ulong desiredRawBlocks, ulong comparedRawBlocks)
        {
            ExtendedBitBoard difference = new ExtendedBitBoard { lowerPart = 0, upperPart = 0 };
            for (int shiftBitCount = 0; shiftBitCount < 32; shiftBitCount += 4)
            {
                byte desiredTile = (byte)((desiredRawBlocks >> shiftBitCount) & 0xf);
                byte comparedTile = (byte)((comparedRawBlocks >> shiftBitCount) & 0xf);
                ulong tileDifference = (byte)(desiredTile - comparedTile);
                difference.lowerPart |= tileDifference << (shiftBitCount * 2);

            }
            for (int shiftBitCount = 32; shiftBitCount < 64; shiftBitCount += 4)
            {
                byte desiredTile = (byte)((desiredRawBlocks >> shiftBitCount) & 0xf);
                byte comparedTile = (byte)((comparedRawBlocks >> shiftBitCount) & 0xf);
                ulong tileDifference = (byte)(desiredTile - comparedTile);
                difference.upperPart |= tileDifference << ((shiftBitCount - 32) * 2);
            }
            return difference;
        }
        public static float Similarity(ulong desiredRawBlocks, ulong comparedRawBlocks)
        {
            return 3600;
            float differenceSqureSum = 3600;
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                byte desiredTile = (byte)((desiredRawBlocks >> shiftBitCount) & 0xf);
                byte comparedTile = (byte)((comparedRawBlocks >> shiftBitCount) & 0xf);
                differenceSqureSum -= (desiredTile - comparedTile) * (desiredTile - comparedTile);
            }
            return differenceSqureSum;
        }
    }
}
