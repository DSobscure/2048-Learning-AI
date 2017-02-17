using Game2048.AI.TD_Learning;
using Game2048.AI.TD_Learning.MultistageTupleNetworks;
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
            TupleNetwork tupleNetwork = null;
            switch (command)
            {
                case "basic":
                    tupleNetwork = new SimpleTupleNetwork("BasicTD", tupleNetworkIndex);
                    tupleNetwork.Load(out loadedCount);
                    agent = new LearningAgent(new TD_LearningAI(0.0025f, tupleNetwork));
                    break;
                case "reward":
                    tupleNetwork = new SimpleTupleNetwork("RewardLearningTD", tupleNetworkIndex);
                    tupleNetwork.Load(out loadedCount);
                    agent = new LearningAgent(new RewardLearningAI(
                        learningRate: 0.0025f,
                        tupleNetwork: tupleNetwork));
                    break;
                case "mlp":
                    tupleNetwork = new SimpleTupleNetwork("MLP_RewardTrained_RewardLearningTD", tupleNetworkIndex);
                    tupleNetwork.Load(out loadedCount);
                    agent = new LearningAgent(new MLP_RewardTrained_RewardLearningAI(
                        learningRate: 0.0025f,
                        rewardPerceptronLearningRate: 0.01f,
                        activationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)),
                        dActivationFunction: (sum) => 1.0f / (1.0f + (float)Math.Exp(-sum)) * (1 - 1.0f / (1.0f + (float)Math.Exp(-sum))),
                        tupleNetwork: tupleNetwork));
                    break;
                case "vr":
                    {
                        tupleNetwork = new SimpleTupleNetwork("VerticalRewardValueTN", tupleNetworkIndex);
                        TupleNetwork rewardTupleNetwork = new SimpleTupleNetwork("VerticalRewardRewardTN", tupleNetworkIndex);

                        int loadedCountExtra1;
                        tupleNetwork.Load(out loadedCount);
                        rewardTupleNetwork.Load(out loadedCountExtra1);
                        loadedCount += loadedCountExtra1;

                        agent = new LearningAgent(new VerticalRewardLearningAI(
                            learningRate: 0.0025f,
                            rewardLearningRate: 0.0025f,
                            tupleNetwork: tupleNetwork,
                            rewardTupleNetwork: rewardTupleNetwork));
                    }
                    break;
                case "hr":
                    {
                        tupleNetwork = new SimpleTupleNetwork("HorizontalRewardValueTN", tupleNetworkIndex);
                        TupleNetwork rewardTupleNetwork = new SimpleTupleNetwork("HorizontalRewardRewardTN", tupleNetworkIndex);

                        int loadedCountExtra1;
                        tupleNetwork.Load(out loadedCount);
                        rewardTupleNetwork.Load(out loadedCountExtra1);
                        loadedCount += loadedCountExtra1;

                        agent = new LearningAgent(new HorizontalRewardLearningAI(
                        learningRate: 0.0025f,
                        rewardLearningRate: 0.0025f,
                        tupleNetwork: tupleNetwork,
                            rewardTupleNetwork: rewardTupleNetwork));
                    }
                    break;
                case "cr":
                    {
                        tupleNetwork = new SimpleTupleNetwork("CrossRewardValueTN", tupleNetworkIndex);
                        tupleNetwork.Load(out loadedCount);
                        int loadedCountExtra1, loadedCountExtra2;
                        TupleNetwork verticalRewardTN = new SimpleTupleNetwork("Cross_VerticalRewardTN", tupleNetworkIndex);
                        TupleNetwork horizontalRewardTN = new SimpleTupleNetwork("Cross_HorizontalRewardTN", tupleNetworkIndex);
                        verticalRewardTN.Load(out loadedCountExtra1);
                        horizontalRewardTN.Load(out loadedCountExtra2);
                        loadedCount += loadedCountExtra1 + loadedCountExtra2;

                        agent = new LearningAgent(new CrossRewardLearningAI(
                            learningRate: 0.0025f,
                            rewardLearningRate: 0.0025f,
                            tupleNetwork: tupleNetwork,
                            verticalRewardTN: verticalRewardTN,
                            horizontalRewardTN: horizontalRewardTN));
                    }
                    break;
                case "ms 0 16384 basic":
                    {
                        tupleNetwork = new TwoStage_0_16384TupleNetwork("BasicMSTN_0_16384", tupleNetworkIndex);
                        tupleNetwork.Load(out loadedCount);
                        agent = new LearningAgent(new TD_LearningAI(0.0025f, tupleNetwork));
                    }
                    break;
                case "ms 0 16384 vr":
                    {
                        tupleNetwork = new TwoStage_0_16384TupleNetwork("VerticalRewardValueMSTN_0_16384", tupleNetworkIndex);
                        TupleNetwork rewardTupleNetwork = new TwoStage_0_16384TupleNetwork("VerticalRewardRewardMSTN_0_16384", tupleNetworkIndex);

                        int loadedCountExtra1;
                        tupleNetwork.Load(out loadedCount);
                        rewardTupleNetwork.Load(out loadedCountExtra1);
                        loadedCount += loadedCountExtra1;

                        agent = new LearningAgent(new VerticalRewardLearningAI(
                            learningRate: 0.0025f,
                            rewardLearningRate: 0.0025f,
                            tupleNetwork: tupleNetwork,
                            rewardTupleNetwork: rewardTupleNetwork));
                    }
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
                case "mlp":
                    agent.Training("MLP_Reward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "vr":
                    agent.Training("VerticalReward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "hr":
                    agent.Training("HorizontalReward", TranningMode.Classical, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "cr":
                    agent.Training("CrossReward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    break;
                case "ms 0 16384 basic":
                    {
                        agent.Training("Multistage_0_16384_Basic", TranningMode.Classical, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    }
                    break;
                case "ms 0 16384 vr":
                    {
                        agent.Training("Multistage_0_16384_VerticalReward", TranningMode.ScoreTrainedReward, 1, 5000000, 1000, ConsoleGameEnvironment.PrintBoard);
                    }
                    break;
                default:
                    Console.WriteLine("Not Existed Command");
                    break;
            }
        }
    }
}
