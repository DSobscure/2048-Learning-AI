using Game2048.AI.TD_Learning;
using Game2048.AI.TD_Learning.TupleFeatures;
using Game2048.AI.GoalBasedLearning;
using Game2048.AI.GoalBasedLearning.ExtendedTupleFeatures;
using System;

namespace Game2048.Game.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            int loadedCount;
            //LearningAgent agent = new LearningAgent(new TD_LearningAI(0.0025f, out loadedCount));
            LearningAgent agent = new LearningAgent(new GoalBasedTD_LearningAI(0.0025f, out loadedCount));
            Console.WriteLine("Loaded {0} TupleFeatures", loadedCount);
            agent.Training(200000, 1000, ConsoleGameEnvironment.PrintBoard);

            Console.Read();
        }
    }
}
