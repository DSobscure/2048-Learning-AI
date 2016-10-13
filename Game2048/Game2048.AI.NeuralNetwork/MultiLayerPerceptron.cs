using System;
using System.Linq;

namespace Game2048.AI.NeuralNetwork
{
    public class MultiLayerPerceptron
    {
        public int InputDimension { get; protected set; }
        public int OutputDimension { get; protected set; }
        public int HiddenLayerNumber { get; protected set; }
        private double[][][] weights;
        public double LearningRate { get; protected set; }
        public Func<double, double> ActivationFunction { get; protected set; }
        public Func<double, double> dActivationFunction { get; protected set; }

        public MultiLayerPerceptron(int inputDimension, int outputDimension, int hiddenLayerNumber, int[] hiddenLayerNodeNumber, double learningRate, Func<double, double> activationFunction, Func<double, double> dActivationFunction)
        {
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            HiddenLayerNumber = hiddenLayerNumber;
            weights = new double[hiddenLayerNumber + 1][][];
            int perviousLayerDimension = inputDimension;
            Random randomGenerator = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < hiddenLayerNumber; i++)
            {
                weights[i] = new double[hiddenLayerNodeNumber[i]][];
                for (int j = 0; j < hiddenLayerNodeNumber[i]; j++)
                {
                    weights[i][j] = new double[perviousLayerDimension + 1];
                    for (int k = 0; k < perviousLayerDimension + 1; k++)
                    {
                        weights[i][j][k] = randomGenerator.NextDouble() * 2 - 1;
                    }
                }
                perviousLayerDimension = hiddenLayerNodeNumber[i];
            }
            weights[hiddenLayerNumber] = new double[outputDimension][];
            for (int i = 0; i < outputDimension; i++)
            {
                weights[hiddenLayerNumber][i] = new double[perviousLayerDimension + 1];
                for (int j = 0; j < perviousLayerDimension + 1; j++)
                {
                    weights[hiddenLayerNumber][i][j] = randomGenerator.NextDouble() * 2 - 1;
                }
            }
            LearningRate = learningRate;
            ActivationFunction = activationFunction;
            this.dActivationFunction = dActivationFunction;
        }
        public int HiddenLayerNodeCount(int layer)
        {
            return weights[layer].Length;
        }

        public double[] Compute(double[] input)
        {
            double[] output = null;
            int perviousLayerDimension = InputDimension;
            for (int layerIndex = 0; layerIndex < HiddenLayerNumber; layerIndex++)
            {
                output = new double[weights[layerIndex].Length];
                for (int nodeIndex = 0; nodeIndex < weights[layerIndex].Length; nodeIndex++)
                {
                    double sum = weights[layerIndex][nodeIndex][perviousLayerDimension];
                    for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                    {
                        sum += weights[layerIndex][nodeIndex][weightsIndex] * input[weightsIndex];
                    }
                    output[nodeIndex] = ActivationFunction(sum);
                }
                perviousLayerDimension = weights[layerIndex].Length;
                input = output;
            }
            output = new double[OutputDimension];
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                double sum = weights[HiddenLayerNumber][nodeIndex][perviousLayerDimension];
                for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                {
                    sum += weights[HiddenLayerNumber][nodeIndex][weightsIndex] * input[weightsIndex];
                }
                output[nodeIndex] = ActivationFunction(sum);
            }
            return output;
        }
        public void Tranning(double[] input, double[] desiredOutput, out double error)
        {
            #region compute sum output
            double[][] nodeSums = new double[HiddenLayerNumber + 1][];
            double[][] nodeDeltas = new double[HiddenLayerNumber + 1][];
            double[] output = null;
            double[][] layerInput = new double[HiddenLayerNumber + 1][];
            int perviousLayerDimension = InputDimension;
            for (int layerIndex = 0; layerIndex < HiddenLayerNumber; layerIndex++)
            {
                var argumentInputVector = input.ToList();
                argumentInputVector.Add(1);
                layerInput[layerIndex] = argumentInputVector.ToArray();
                int nodeCount = weights[layerIndex].Length;
                output = new double[nodeCount];
                nodeSums[layerIndex] = new double[nodeCount];
                nodeDeltas[layerIndex] = new double[nodeCount];
                for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
                {
                    double sum = weights[layerIndex][nodeIndex][perviousLayerDimension];
                    for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                    {
                        sum += weights[layerIndex][nodeIndex][weightsIndex] * input[weightsIndex];
                    }
                    nodeSums[layerIndex][nodeIndex] = sum;
                    output[nodeIndex] = ActivationFunction(sum);
                }
                perviousLayerDimension = nodeCount;
                input = output;
            }
            var argumentInputVectorFinal = input.ToList();
            argumentInputVectorFinal.Add(1);
            layerInput[HiddenLayerNumber] = argumentInputVectorFinal.ToArray();
            output = new double[OutputDimension];
            nodeSums[HiddenLayerNumber] = new double[OutputDimension];
            nodeDeltas[HiddenLayerNumber] = new double[OutputDimension];
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                double sum = weights[HiddenLayerNumber][nodeIndex][perviousLayerDimension];
                for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                {
                    sum += weights[HiddenLayerNumber][nodeIndex][weightsIndex] * input[weightsIndex];
                }
                nodeSums[HiddenLayerNumber][nodeIndex] = sum;
                output[nodeIndex] = ActivationFunction(sum);
            }
            #endregion
            error = 0;
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                error += (desiredOutput[nodeIndex] - output[nodeIndex]) * (desiredOutput[nodeIndex] - output[nodeIndex]);
                nodeDeltas[HiddenLayerNumber][nodeIndex] = (desiredOutput[nodeIndex] - output[nodeIndex]) * dActivationFunction(nodeSums[HiddenLayerNumber][nodeIndex]);
            }
            error = Math.Sqrt(error);
            for (int layerIndex = HiddenLayerNumber - 1; layerIndex >= 0; layerIndex--)
            {
                for (int nodeIndex = 0; nodeIndex < weights[layerIndex].Length; nodeIndex++)
                {
                    double deltaSum = 0;
                    for (int previousLayerNodeIndex = 0; previousLayerNodeIndex < weights[layerIndex + 1].Length; previousLayerNodeIndex++)
                    {
                        deltaSum += nodeDeltas[layerIndex + 1][previousLayerNodeIndex] * weights[layerIndex + 1][previousLayerNodeIndex][nodeIndex];
                    }
                    nodeDeltas[layerIndex][nodeIndex] = deltaSum * dActivationFunction(nodeSums[layerIndex][nodeIndex]);
                }
            }

            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                for (int weightsIndex = 0; weightsIndex < weights[HiddenLayerNumber][nodeIndex].Length; weightsIndex++)
                {
                    weights[HiddenLayerNumber][nodeIndex][weightsIndex] += LearningRate * nodeDeltas[HiddenLayerNumber][nodeIndex] * layerInput[HiddenLayerNumber][weightsIndex];
                }
            }
            for (int layerIndex = HiddenLayerNumber - 1; layerIndex >= 0; layerIndex--)
            {
                for (int nodeIndex = 0; nodeIndex < weights[layerIndex].Length; nodeIndex++)
                {
                    for (int weightsIndex = 0; weightsIndex < weights[layerIndex][nodeIndex].Length; weightsIndex++)
                    {
                        weights[layerIndex][nodeIndex][weightsIndex] += LearningRate * nodeDeltas[layerIndex][nodeIndex] * layerInput[layerIndex][weightsIndex];
                    }
                }
            }
        }
    }
}
