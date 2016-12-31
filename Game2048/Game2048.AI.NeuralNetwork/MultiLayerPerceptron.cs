using MsgPack.Serialization;
using System;
using System.Linq;

namespace Game2048.AI.NeuralNetwork
{
    public class MultiLayerPerceptron
    {
        [MessagePackMember(id:0, Name = "InputDimension")]
        public int InputDimension { get; protected set; }
        [MessagePackMember(id: 1, Name = "OutputDimension")]
        public int OutputDimension { get; protected set; }
        [MessagePackMember(id: 2, Name = "HiddenLayerNumber")]
        public int HiddenLayerNumber { get; protected set; }
        [MessagePackMember(id: 3, Name = "weights")]
        private float[][][] weights;
        [MessagePackMember(id: 4, Name = "LearningRate")]
        public float LearningRate { get; set; }
        public Func<float, float> ActivationFunction { get; protected set; }
        public Func<float, float> dActivationFunction { get; protected set; }

        [MessagePackDeserializationConstructor]
        public MultiLayerPerceptron()
        {
            ActivationFunction = (sum) => Math.Max(sum, 0);
            dActivationFunction = (sum) => (sum > 0) ? 1 : 0;
        }
        public MultiLayerPerceptron(int inputDimension, int outputDimension, int hiddenLayerNumber, int[] hiddenLayerNodeNumber, float learningRate, Func<float, float> activationFunction, Func<float, float> dActivationFunction)
        {
            InputDimension = inputDimension;
            OutputDimension = outputDimension;
            HiddenLayerNumber = hiddenLayerNumber;
            weights = new float[hiddenLayerNumber + 1][][];
            int perviousLayerDimension = inputDimension;
            Random randomGenerator = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < hiddenLayerNumber; i++)
            {
                weights[i] = new float[hiddenLayerNodeNumber[i]][];
                for (int j = 0; j < hiddenLayerNodeNumber[i]; j++)
                {
                    weights[i][j] = new float[perviousLayerDimension + 1];
                    for (int k = 0; k < perviousLayerDimension + 1; k++)
                    {
                        weights[i][j][k] = (float)(randomGenerator.NextDouble() * 2 - 1);
                    }
                }
                perviousLayerDimension = hiddenLayerNodeNumber[i];
            }
            weights[hiddenLayerNumber] = new float[outputDimension][];
            for (int i = 0; i < outputDimension; i++)
            {
                weights[hiddenLayerNumber][i] = new float[perviousLayerDimension + 1];
                for (int j = 0; j < perviousLayerDimension + 1; j++)
                {
                    weights[hiddenLayerNumber][i][j] = (float)(randomGenerator.NextDouble() * 2 - 1);
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

        public unsafe float[] Compute(float[] input)
        {
            float[] output = null;
            int perviousLayerDimension = InputDimension;
            for (int layerIndex = 0; layerIndex < HiddenLayerNumber; layerIndex++)
            {
                output = new float[weights[layerIndex].Length];
                int weightsLength = weights[layerIndex].Length;
                for (int nodeIndex = 0; nodeIndex < weightsLength; nodeIndex++)
                {
                    fixed (float* weightsPointer = weights[layerIndex][nodeIndex])
                    {
                        float sum = weightsPointer[perviousLayerDimension];
                        for (int weightsIndex = 0; weightsIndex != perviousLayerDimension; weightsIndex++)
                        {
                            sum += weightsPointer[weightsIndex] * input[weightsIndex];
                        }
                        output[nodeIndex] = ActivationFunction(sum);
                    }
                }
                perviousLayerDimension = weights[layerIndex].Length;
                input = output;
            }
            output = new float[OutputDimension];
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                fixed (float* weightsPointer = weights[HiddenLayerNumber][nodeIndex])
                {
                    float sum = weightsPointer[perviousLayerDimension];
                    for (int weightsIndex = 0; weightsIndex != perviousLayerDimension; weightsIndex++)
                    {
                        sum += weightsPointer[weightsIndex] * input[weightsIndex];
                    }
                    output[nodeIndex] = ActivationFunction(sum);
                }
            }
            return output;
        }
        public unsafe void Tranning(float[] input, float[] desiredOutput, out float error)
        {
            #region compute sum output
            float[][] nodeSums = new float[HiddenLayerNumber + 1][];
            float[][] nodeDeltas = new float[HiddenLayerNumber + 1][];
            float[] output = null;
            float[][] layerInput = new float[HiddenLayerNumber + 1][];
            int perviousLayerDimension = InputDimension;
            for (int layerIndex = 0; layerIndex < HiddenLayerNumber; layerIndex++)
            {
                var argumentInputVector = input.ToList();
                argumentInputVector.Add(1);
                layerInput[layerIndex] = argumentInputVector.ToArray();
                int nodeCount = weights[layerIndex].Length;
                output = new float[nodeCount];
                nodeSums[layerIndex] = new float[nodeCount];
                nodeDeltas[layerIndex] = new float[nodeCount];
                fixed(float* nodeSumsPointer = nodeSums[layerIndex])
                for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
                {
                    fixed(float* weightsPointer = weights[layerIndex][nodeIndex])
                    {
                        float sum = weightsPointer[perviousLayerDimension];
                        for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                        {
                            sum += weightsPointer[weightsIndex] * input[weightsIndex];
                        }
                        nodeSumsPointer[nodeIndex] = sum;
                        output[nodeIndex] = ActivationFunction(sum);
                    }
                }
                perviousLayerDimension = nodeCount;
                input = output;
            }
            var argumentInputVectorFinal = input.ToList();
            argumentInputVectorFinal.Add(1);
            layerInput[HiddenLayerNumber] = argumentInputVectorFinal.ToArray();
            output = new float[OutputDimension];
            nodeSums[HiddenLayerNumber] = new float[OutputDimension];
            nodeDeltas[HiddenLayerNumber] = new float[OutputDimension];
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                fixed (float* weightsPointer = weights[HiddenLayerNumber][nodeIndex])
                {
                    float sum = weightsPointer[perviousLayerDimension];
                    fixed(float* inputPointer = input)
                    for (int weightsIndex = 0; weightsIndex < perviousLayerDimension; weightsIndex++)
                    {
                        sum += weightsPointer[weightsIndex] * inputPointer[weightsIndex];
                    }
                    nodeSums[HiddenLayerNumber][nodeIndex] = sum;
                    output[nodeIndex] = ActivationFunction(sum);
                }
            }
            #endregion
            error = 0;
            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                error += (desiredOutput[nodeIndex] - output[nodeIndex]) * (desiredOutput[nodeIndex] - output[nodeIndex]);
                nodeDeltas[HiddenLayerNumber][nodeIndex] = (desiredOutput[nodeIndex] - output[nodeIndex]) * dActivationFunction(nodeSums[HiddenLayerNumber][nodeIndex]);
            }
            error = (float)Math.Sqrt(error);
            for (int layerIndex = HiddenLayerNumber - 1; layerIndex >= 0; layerIndex--)
            {
                float[] nodeDeltasCache = nodeDeltas[layerIndex];
                float[] nodeSumsCache = nodeSums[layerIndex];
                float[][] weightsCache = weights[layerIndex + 1];
                for (int nodeIndex = 0; nodeIndex < weights[layerIndex].Length; nodeIndex++)
                {
                    float deltaSum = 0;
                    int weightsLength = weights[layerIndex + 1].Length;
                    fixed(float* nodeDeltasPointer = nodeDeltas[layerIndex + 1])
                    {
                        for (int previousLayerNodeIndex = 0; previousLayerNodeIndex != weightsLength; previousLayerNodeIndex++)
                        {
                            deltaSum += nodeDeltasPointer[previousLayerNodeIndex] * weightsCache[previousLayerNodeIndex][nodeIndex];
                        }
                    }
                    nodeDeltasCache[nodeIndex] = deltaSum * dActivationFunction(nodeSumsCache[nodeIndex]);
                }
            }

            for (int nodeIndex = 0; nodeIndex < OutputDimension; nodeIndex++)
            {
                int weightLength = weights[HiddenLayerNumber][nodeIndex].Length;
                fixed (float* weightsPointer = weights[HiddenLayerNumber][nodeIndex])
                {
                    for (int weightsIndex = 0; weightsIndex < weightLength; weightsIndex++)
                    {
                        weightsPointer[weightsIndex] += LearningRate * nodeDeltas[HiddenLayerNumber][nodeIndex] * layerInput[HiddenLayerNumber][weightsIndex];
                    }
                }
            }
            for (int layerIndex = HiddenLayerNumber - 1; layerIndex >= 0; layerIndex--)
            {
                int nodeLength = weights[layerIndex].Length;
                for (int nodeIndex = 0; nodeIndex < nodeLength; nodeIndex++)
                {
                    int weightLength = weights[layerIndex][nodeIndex].Length;
                    float nodeDelta = nodeDeltas[layerIndex][nodeIndex];

                    fixed (float* weightsPointer = weights[layerIndex][nodeIndex])
                    fixed(float* layerInputPointer = layerInput[layerIndex])
                    {
                        for (int weightsIndex = 0; weightsIndex < weightLength; weightsIndex++)
                        {
                            weightsPointer[weightsIndex] += LearningRate * nodeDelta * layerInputPointer[weightsIndex];
                        }
                    }
                }
            }
        }
    }
}
