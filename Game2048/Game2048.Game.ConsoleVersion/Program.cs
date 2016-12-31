using Game2048.AI.TD_Learning;
using System;

namespace Game2048.Game.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            int loadedCount;
            LearningAgent agent = new LearningAgent(new RewardLearningAI(
                learningRate: 0.0001f,
                rewardPerceptronLearningRate: 0.1f,
                activationFunction: (sum) => Math.Max(sum, 0),
                dActivationFunction: (sum) => (sum > 0) ? 1 : 0,
                loadedCount: out loadedCount));
            //LearningAgent agent = new LearningAgent(new StepEvaluationLearningAI(0.00005f, out loadedCount));
            //LearningAgent agent = new LearningAgent(new GoalBasedTD_LearningAI(0.0025f, out loadedCount));
            Console.WriteLine("Loaded {0} TupleFeatures", loadedCount);
            agent.Training(10000000, 1000, ConsoleGameEnvironment.PrintBoard);

            Console.Read();
        }
    }
}
