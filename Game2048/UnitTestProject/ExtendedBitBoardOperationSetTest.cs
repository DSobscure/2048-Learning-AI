using Game2048.AI.GoalBasedLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ExtendedBitBoardOperationSetTest
    {
        [TestMethod]
        public void ExtendedBitBoardOperationSetTest_GetRow()
        {
            ExtendedBitBoard board = new ExtendedBitBoard { upperPart = 0x0001000200030004, lowerPart = 0x0005000600070008 };
            Assert.AreEqual(ExtendedBitBoardOperationSet.GetRow(board, 0), 0x10002u);
            Assert.AreEqual(ExtendedBitBoardOperationSet.GetRow(board, 1), 0x30004u);
            Assert.AreEqual(ExtendedBitBoardOperationSet.GetRow(board, 2), 0x50006u);
            Assert.AreEqual(ExtendedBitBoardOperationSet.GetRow(board, 3), 0x70008u);
        }
        [TestMethod]
        public unsafe void ExtendedBitBoardOperationSetTest_SetRows()
        {
            uint* numbers = stackalloc uint[4];
            numbers[0] = 0x00010002;
            numbers[1] = 0x00030004;
            numbers[2] = 0x00050006;
            numbers[3] = 0x00070008;
            ExtendedBitBoard board = new ExtendedBitBoard { upperPart = 0x0001000200030004, lowerPart = 0x0005000600070008 };
            Assert.AreEqual(ExtendedBitBoardOperationSet.SetRows(numbers), board);
        }
        [TestMethod]
        public void ExtendedBitBoardOperationSetTest_Transpose()
        {
            ExtendedBitBoard board = new ExtendedBitBoard { upperPart = 0x0000005100000062, lowerPart = 0x0000007300000084 };
            ExtendedBitBoard transposedBoard = new ExtendedBitBoard { upperPart = 0, lowerPart = 0x0000000051627384 };
            Assert.AreEqual(ExtendedBitBoardOperationSet.Transpose(board), transposedBoard);
        }
        [TestMethod]
        public unsafe void ExtendedBitBoardOperationSetTest_SetColumns()
        {
            uint* numbers = stackalloc uint[4];
            numbers[0] = 0x00000012;
            numbers[1] = 0x00000034;
            numbers[2] = 0x00000056;
            numbers[3] = 0x00000078;
            ExtendedBitBoard board = new ExtendedBitBoard { upperPart = 0, lowerPart = 0x0000000012345678 };
            Assert.AreEqual(ExtendedBitBoardOperationSet.SetColumns(numbers), board);
        }
    }
}
