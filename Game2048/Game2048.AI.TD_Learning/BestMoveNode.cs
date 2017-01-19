using Game2048.Game.Library;

namespace Game2048.AI.TD_Learning
{
    public struct BestMoveNode
    {
        public Direction bestMove;
        public float reward;
        public ulong rawBlocks;
        public ulong movedRawBlocks;
    };
}
