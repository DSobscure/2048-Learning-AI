namespace Game2048.Game.Library.TableSets
{
    public struct RowShiftInfo
    {
        public ushort row;
        public int reward;
    }
    public struct ColumnShiftInfo
    {
        public ulong column;
        public int reward;
    }
    public static class BitBoardShiftTableSet
    {
        public static readonly RowShiftInfo[] rowShiftLeftTable;
        public static readonly RowShiftInfo[] rowShiftRightTable;
        public static readonly ColumnShiftInfo[] columnShiftUpTable;
        public static readonly ColumnShiftInfo[] columnShiftDownTable;

        static BitBoardShiftTableSet()
        {
            rowShiftLeftTable = new RowShiftInfo[65536];
            rowShiftRightTable = new RowShiftInfo[65536];
            columnShiftUpTable = new ColumnShiftInfo[65536];
            columnShiftDownTable = new ColumnShiftInfo[65536];

            for (int rowContent = 0; rowContent < 65536; rowContent++)
            {
                int reward = 0;
                int[] lines =
                {
                   (rowContent >>  0) & 0xf,
                   (rowContent >>  4) & 0xf,
                   (rowContent >>  8) & 0xf,
                   (rowContent >> 12) & 0xf
                };
                // execute a move to the left
                for (int blockIndex = 0; blockIndex < 3; blockIndex++)
                {
                    int firstNonEmptyBlockIndex = blockIndex + 1;
                    while(firstNonEmptyBlockIndex < 4 && lines[firstNonEmptyBlockIndex] == 0)
                    {
                        firstNonEmptyBlockIndex++;
                    }
                    if (firstNonEmptyBlockIndex > 3)
                        break;

                    if (lines[blockIndex] == 0)
                    {
                        lines[blockIndex] = lines[firstNonEmptyBlockIndex];
                        lines[firstNonEmptyBlockIndex] = 0;
                        blockIndex--;
                    }
                    else if (lines[blockIndex] == lines[firstNonEmptyBlockIndex])
                    {
                        if (lines[blockIndex] != 0xf)
                        {
                            lines[blockIndex]++;
                            lines[firstNonEmptyBlockIndex] = 0;
                            reward += 2 << (lines[blockIndex] - 1);
                        }
                    }
                }

                int reversedRowContent = BitBoardOperationSet.ReverseRow(rowContent);
                int resultRowContent = (lines[0] << 0) | (lines[1] << 4) | (lines[2] << 8) | (lines[3] << 12);
                int reversedResultRowContent = BitBoardOperationSet.ReverseRow(resultRowContent);

                rowShiftLeftTable[rowContent] = new RowShiftInfo { row = (ushort)(rowContent ^ resultRowContent), reward = reward };
                rowShiftRightTable[reversedRowContent] = new RowShiftInfo { row = (ushort)(reversedRowContent ^ reversedResultRowContent), reward = reward };
                columnShiftUpTable[rowContent] = new ColumnShiftInfo { column = BitBoardOperationSet.UnpackColumn(rowContent) ^ BitBoardOperationSet.UnpackColumn(resultRowContent), reward = reward };
                columnShiftDownTable[reversedRowContent] = new ColumnShiftInfo { column = BitBoardOperationSet.UnpackColumn(reversedRowContent) ^ BitBoardOperationSet.UnpackColumn(reversedResultRowContent), reward = reward };
            }
        }
    }
}
