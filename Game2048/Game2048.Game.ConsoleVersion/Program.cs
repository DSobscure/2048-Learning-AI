﻿using Game2048.AI.TD_Learning;
using Game2048.AI.NeuralNetwork;
using System;

namespace Game2048.Game.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input Mode");
            string command = Console.ReadLine();
            Console.WriteLine("TupleNetworkIndex");
            int tupleNetworkIndex = int.Parse(Console.ReadLine());
            int loadedCount = 0;
            LearningAgent agent = null;
            switch (command)
            {
                case "basic":
                    agent = new LearningAgent(new TD_LearningAI(0.0025f, out loadedCount, tupleNetworkIndex));
                    break;
                case "reward":
                    agent = new LearningAgent(new RewardLearningAI(
                        learningRate: 0.0025f,
                        loadedCount: out loadedCount,
                        tupleNetworkIndex: tupleNetworkIndex));
                    break;
                case "mlp score reward":
                    agent = new LearningAgent(new MLP_RewardTrained_RewardLearningAI(
                        learningRate: 0.0025f,
                        rewardPerceptronLearningRate: 0.01f,
                        activationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)),
                        dActivationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)) * (1 - 1.0f / (1.0f + (float)Math.Exp(-sum))),
                        loadedCount: out loadedCount,
                        tupleNetworkIndex: tupleNetworkIndex));
                    break;
                case "mlp step reward":
                    agent = new LearningAgent(new MLP_RewardTrained_RewardLearningAI(
                        learningRate: 0.0025f,
                        rewardPerceptronLearningRate: 0.01f,
                        activationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)),
                        dActivationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)) * (1 - 1.0f / (1.0f + (float)Math.Exp(-sum))),
                        loadedCount: out loadedCount,
                        tupleNetworkIndex: tupleNetworkIndex));
                    break;
                case "tn score":
                    agent = new LearningAgent(new TN_RewardTrained_RewardLearningAI(
                        learningRate: 0.0025f,
                        rewardLearningRate: 0.01f,
                        loadedCount: out loadedCount,
                        tupleNetworkIndex: tupleNetworkIndex));
                    break;
                case "tn step":
                    agent = new LearningAgent(new TN_RewardTrained_RewardLearningAI(
                        learningRate: 0.0025f,
                        rewardLearningRate: 0.01f,
                        loadedCount: out loadedCount,
                        tupleNetworkIndex: tupleNetworkIndex));
                    break;
                default:
                    Console.WriteLine("Not Existed Command");
                    break;
            }
            Console.WriteLine("Loaded {0} TupleFeatures", loadedCount);
            switch (command)
            {
                case "basic":
                    agent.Training("Basic", TranningMode.Classical, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "reward":
                    agent.Training("Reward", TranningMode.Classical, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "mlp score reward":
                    agent.Training("MLP Score Reward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "mlp step reward":
                    agent.Training("MLP Step Reward", TranningMode.StepTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "tn score":
                    agent.Training("TN Score Reward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "tn step":
                    agent.Training("TN Step Reward", TranningMode.StepTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                default:
                    Console.WriteLine("Not Existed Command");
                    break;
            }
        }
    }
}
