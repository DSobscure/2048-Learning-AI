using System;
using System.Collections.Generic;
using System.IO;
using Game2048.Game.Library;

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
        public string SerializationFileName { get; private set; }

        public EndgameClassifier(string serializationFileName)
        {
            SerializationFileName = serializationFileName;

            if (File.Exists(serializationFileName))
            {
                classifier = SerializationHelper.Deserialize<MultiLayerPerceptron>(File.ReadAllBytes(serializationFileName));
            }
            else
            {
                Func<double, double> activationFunction = (input) =>
                {
                    return 1.0 / (1.0 + Math.Exp(-input));
                };
                Func<double, double> dActivationFunction = (input) =>
                {
                    return activationFunction(input) * (1 - activationFunction(input));
                };
                classifier = new MultiLayerPerceptron(16, 1, 5, new int[] { 64, 64, 64, 16, 4 }, 0.01, activationFunction, dActivationFunction);
            }
        }

        public void TrainContinuousEndgameBoards(List<ulong> rawBoardRecords, int startIndex, out double errorRate)
        {
            double[] isEndgameVector = new double[1] { 1 };
            double[] notEndgameVector = new double[1] { 0 };
            double error;
            double totalError = 0;
            for (int i = 0; i < startIndex; i++)
            {
                classifier.Tranning(ExtractFeature(rawBoardRecords[i]), notEndgameVector, out error);
                totalError += error;
            }
            for (int i = startIndex; i < rawBoardRecords.Count; i++)
            {
                classifier.Tranning(ExtractFeature(rawBoardRecords[i]), isEndgameVector, out error);
                totalError += error;
            }
            errorRate = totalError / rawBoardRecords.Count;
        }
        public void TrainDiscreteEndgameBoards(ulong rawBoard, bool isEndgame, out double error)
        {
            double[] inputVector = (isEndgame) ? new double[1] { 1 } : new double[1] { 0 };
            classifier.Tranning(ExtractFeature(rawBoard), inputVector, out error);
        }
        public bool IsEndgame(ulong rawBlocks)
        {
            double result = classifier.Compute(ExtractFeature(rawBlocks))[0];
            return result > 0.5;
        }

        public void SaveClassifier()
        {
            File.WriteAllBytes(SerializationFileName, SerializationHelper.Serialize(classifier as MultiLayerPerceptron));
        }
    }
}
