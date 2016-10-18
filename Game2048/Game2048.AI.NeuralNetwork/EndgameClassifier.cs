using System;
using System.Collections.Generic;

namespace Game2048.AI.NeuralNetwork
{
    public class EndgameClassifier
    {
        private static double[] ExtractFeature(ulong rawBlocks)
        {
            double[] result = new double[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = (rawBlocks >> (i * 4)) & 0xf;
            }
            return result;
        }

        private MultiLayerPerceptron classifier;

        public EndgameClassifier()
        {
            Func<double, double> activationFunction = (input) =>
            {
                return 1.0 / (1.0 + Math.Exp(-input));
            };
            Func<double, double> dActivationFunction = (input) =>
            {
                return activationFunction(input) * (1 - activationFunction(input));
            };
            classifier = new MultiLayerPerceptron(16, 1, 3, new int[] { 100, 40, 8 }, 1, activationFunction, dActivationFunction);
        }

        public void TrainContinuousEndgameBoards(List<ulong> rawBlocksRecords, int startIndex, out double errorRate)
        {
            double[] isEndgameVector = new double[1] { 1 };
            double[] notEndgameVector = new double[1] { 0 };
            double error;
            double totalError = 0;
            for (int i = 0; i < startIndex; i++)
            {
                classifier.Tranning(ExtractFeature(rawBlocksRecords[i]), notEndgameVector, out error);
                totalError += error;
            }
            for (int i = startIndex; i < rawBlocksRecords.Count; i++)
            {
                classifier.Tranning(ExtractFeature(rawBlocksRecords[i]), isEndgameVector, out error);
                totalError += error;
            }
            errorRate = totalError / rawBlocksRecords.Count;
        }
        public bool IsEndgame(ulong rawBlocks)
        {
            double result = classifier.Compute(ExtractFeature(rawBlocks))[0];
            return result > 0.5;
        }
    }
}
