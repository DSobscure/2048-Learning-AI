using Game2048.AI.GoalBasedLearning;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class BoardDifferenceComputerTest
    {
        [TestMethod]
        public void TestMethod_Compute_Default()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0), new ExtendedBitBoard { lowerPart = 0, upperPart = 0 });
        }
        [TestMethod]
        public void TestMethod_Compute_1()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0x1, 0), new ExtendedBitBoard { lowerPart = 0, upperPart = 0x1 });
        }
        [TestMethod]
        public void TestMethod_Compute_2()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0x1), new ExtendedBitBoard { lowerPart = 0, upperPart = 0xff });
        }
        [TestMethod]
        public void TestMethod_Compute_3()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0xfedcba9876543210, 0), 
                new ExtendedBitBoard
                {
                    lowerPart = 0x0f0e0d0c0b0a0908,
                    upperPart = 0x0706050403020100
                }
            );
        }
        [TestMethod]
        public void TestMethod_Compute_4()
        {
            Assert.AreEqual(BoardDifferenceComputer.Compute(0, 0xfedcba9876543210),
                new ExtendedBitBoard
                {
                    lowerPart = 0xf1f2f3f4f5f6f7f8,
                    upperPart = 0xf9fafbfcfdfeff00
                }
            );
        }
    }
}
