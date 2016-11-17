using Game2048.AI.GoalBasedLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class BoardDifferenceComputerTest
    {
        [TestMethod]
        public void BoardDifferenceComputerTestMethod_Compute_Default()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0), new ExtendedBitBoard { upperPart = 0, lowerPart = 0 });
        }
        [TestMethod]
        public void BoardDifferenceComputerTestMethod_Compute_1()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0x1, 0), new ExtendedBitBoard { upperPart = 0, lowerPart = 0x1 });
        }
        [TestMethod]
        public void BoardDifferenceComputerTestMethod_Compute_2()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0x1), new ExtendedBitBoard { upperPart = 0, lowerPart = 0xff });
        }
        [TestMethod]
        public void BoardDifferenceComputerTestMethod_Compute_3()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0xfedcba9876543210, 0), 
                new ExtendedBitBoard
                {
                    upperPart = 0x0f0e0d0c0b0a0908,
                    lowerPart = 0x0706050403020100
                }
            );
        }
        [TestMethod]
        public void BoardDifferenceComputerTestMethod_Compute_4()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0xfedcba9876543210),
                new ExtendedBitBoard
                {
                    upperPart = 0xf1f2f3f4f5f6f7f8,
                    lowerPart = 0xf9fafbfcfdfeff00
                }
            );
        }
    }
}
