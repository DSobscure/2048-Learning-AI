using Game2048.AI.GoalBasedLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ExtendedBitBoardOperatorTest
    {
        [TestMethod]
        public void ExtendedBitBoardOperatorTestMethod_AndOperator()
        {
            Assert.AreEqual(new ExtendedBitBoard { upperPart = 0xFFFFFFFFFFFFFFFF, lowerPart = 0xFFFFFFFFFFFFFFFF } & new ExtendedBitBoard { upperPart = 0xF0F0F0F0F0F0F0F0, lowerPart = 0x0F0F0F0F0F0F0F0F },
                new ExtendedBitBoard { upperPart = 0xF0F0F0F0F0F0F0F0, lowerPart = 0x0F0F0F0F0F0F0F0F }
            );
        }
        [TestMethod]
        public void ExtendedBitBoardOperatorTestMethod_OrOperator()
        {
            Assert.AreEqual(new ExtendedBitBoard { upperPart = 0xFFFF0000FFFF0000, lowerPart = 0x00FF00FF00FF00FF } | new ExtendedBitBoard { upperPart = 0x0000111111112222, lowerPart = 0x1212121212121212 },
                new ExtendedBitBoard { upperPart = 0xFFFF1111FFFF2222, lowerPart = 0x12FF12FF12FF12FF }
            );
        }
        [TestMethod]
        public void ExtendedBitBoardOperatorTestMethod_ShiftLeftOperator()
        {
            Assert.AreEqual(new ExtendedBitBoard { upperPart = 0, lowerPart = 0xFFFFFFFFFFFFFFFF } << 24,
                new ExtendedBitBoard { upperPart = 0xFFFFFF, lowerPart = 0xFFFFFFFFFF000000 }
            );
        }
        [TestMethod]
        public void ExtendedBitBoardOperatorTestMethod_ShiftRightOperator()
        {
            Assert.AreEqual(new ExtendedBitBoard { upperPart = 0xFFFFFFFFFFFFFFFF, lowerPart = 0 } >> 24,
                new ExtendedBitBoard { upperPart = 0x000000FFFFFFFFFF, lowerPart = 0xFFFFFF0000000000 }
            );
        }
    }
}
