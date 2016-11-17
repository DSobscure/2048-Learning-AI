namespace Game2048.AI.GoalBasedLearning
{
    public struct ExtendedBitBoard
    {
        public static ExtendedBitBoard operator &(ExtendedBitBoard left, ExtendedBitBoard right)
        {
            return new ExtendedBitBoard
            {
                upperPart = left.upperPart & right.upperPart ,
                lowerPart =  left.lowerPart & right.lowerPart
            };
        }
        public static ExtendedBitBoard operator |(ExtendedBitBoard left, ExtendedBitBoard right)
        {
            return new ExtendedBitBoard
            {
                upperPart = left.upperPart | right.upperPart,
                lowerPart = left.lowerPart | right.lowerPart
            };
        }
        public static ExtendedBitBoard operator <<(ExtendedBitBoard bitBoard, int shiftScale)
        {
            int inverseShiftScale = 64 - shiftScale;
            ulong lowerMask = 0xFFFFFFFFFFFFFFFF << inverseShiftScale;
            return new ExtendedBitBoard
            {
                upperPart = bitBoard.upperPart << shiftScale | ((bitBoard.lowerPart & lowerMask) >> inverseShiftScale),
                lowerPart = bitBoard.lowerPart << shiftScale
            };
        }
        public static ExtendedBitBoard operator >>(ExtendedBitBoard bitBoard, int shiftScale)
        {
            int inverseShiftScale = 64 - shiftScale;
            ulong upperMask = 0xFFFFFFFFFFFFFFFF >> inverseShiftScale;
            return new ExtendedBitBoard
            {
                upperPart = bitBoard.upperPart >> shiftScale,
                lowerPart = bitBoard.lowerPart >> shiftScale | ((bitBoard.upperPart & upperMask) << inverseShiftScale)
            };
        }

        public ulong upperPart;
        public ulong lowerPart;
    }
}
