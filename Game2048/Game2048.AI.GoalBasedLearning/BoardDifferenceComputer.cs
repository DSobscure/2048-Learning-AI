using Game2048.Game.Library;

namespace Game2048.AI.GoalBasedLearning
{
    public static class BoardDifferenceComputer
    {
        public static ExtendedBitBoard Compute(ulong desiredRawBlocks, ulong comparedRawBlocks)
        {
            ExtendedBitBoard difference = new ExtendedBitBoard { lowerPart = 0, upperPart = 0 };
            for (int shiftBitCount = 0; shiftBitCount < 32; shiftBitCount += 4)
            {
                byte desiredTile = (byte)((desiredRawBlocks >> shiftBitCount) & 0xf);
                byte comparedTile = (byte)((comparedRawBlocks >> shiftBitCount) & 0xf);
                ulong tileDifference = (byte)(desiredTile - comparedTile);
                difference.upperPart |= tileDifference << (shiftBitCount * 2);

            }
            for (int shiftBitCount = 32; shiftBitCount < 64; shiftBitCount += 4)
            {
                byte desiredTile = (byte)((desiredRawBlocks >> shiftBitCount) & 0xf);
                byte comparedTile = (byte)((comparedRawBlocks >> shiftBitCount) & 0xf);
                ulong tileDifference = (byte)(desiredTile - comparedTile);
                difference.lowerPart |= tileDifference << ((shiftBitCount - 32) * 2);
            }
            return difference;
        }
    }
}
