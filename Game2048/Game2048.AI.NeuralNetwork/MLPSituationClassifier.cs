using System;
using System.Collections.Generic;
using System.IO;
using Game2048.Game.Library;

namespace Game2048.AI.NeuralNetwork
{
    public class MLPSituationClassifier
    {
        private static float[] ExtractFeature(ulong rawBlocks)
        {
            rawBlocks = BitBoard.InversBlocks(rawBlocks);
            float[] result = new float[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = ((rawBlocks >> (i * 4)) & 0xf);
            }
            return result;
        }

        private MultiLayerPerceptron classifier;
        public string SerializationFileName { get; private set; }

        public MLPSituationClassifier(string serializationFileName)
        {
            SerializationFileName = serializationFileName;

            if (File.Exists(serializationFileName))
            {
                classifier = SerializationHelper.Deserialize<MultiLayerPerceptron>(File.ReadAllBytes(serializationFileName));
            }
            else
            {
                Func<float, float> activationFunction = (input) =>
                {
                    return 1 / (float)(1.0 + Math.Exp(-input));
                };
                Func<float, float> dActivationFunction = (input) =>
                {
                    return activationFunction(input) * (1 - activationFunction(input));
                };
                classifier = new MultiLayerPerceptron(16, 1, 5, new int[] { 64, 64, 64, 16, 4 }, 0.01f, activationFunction, dActivationFunction);
            }
        }
        public void TrainSituation(ulong rawBoard, bool isInSituation, out float error)
        {
            float[] inputVector = (isInSituation) ? new float[1] { 1 } : new float[1] { 0 };
            classifier.Tranning(ExtractFeature(rawBoard), inputVector, out error);
        }
        public bool IsInSituation(ulong rawBlocks)
        {
            return classifier.Compute(ExtractFeature(rawBlocks))[0] > 0.5f;
        }

        public void SaveClassifier()
        {
            File.WriteAllBytes(SerializationFileName, SerializationHelper.Serialize(classifier as MultiLayerPerceptron));
        }
    }
}
