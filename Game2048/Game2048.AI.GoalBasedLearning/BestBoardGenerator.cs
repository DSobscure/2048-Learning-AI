using System;
using Game2048.Game.Library;

namespace Game2048.AI.GoalBasedLearning
{
    public static class BestBoardGenerator
    {
        public static ulong BasicLayeredTile(ulong rawBoard)
        {
            ulong result = 0;
            ulong maxtile = (ulong)BitBoard.RawMaxTileTest(rawBoard);
            for (int shiftBitCount = 0; shiftBitCount < 64; shiftBitCount += 4)
            {
                result |= (ulong)1 << shiftBitCount;
            }

            return result;
        }
    }
}
