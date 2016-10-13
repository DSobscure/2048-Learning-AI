﻿using Game2048.Game.Library;

namespace Game2048.AI.TD_Learning
{
    public struct BestMoveNode
    {
        public Direction bestMove;
        public int reward;
        public ulong movedRawBlocks;
    };
}
