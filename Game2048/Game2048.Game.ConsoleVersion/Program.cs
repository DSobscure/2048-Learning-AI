using Game2048.AI.TD_Learning;
using System;

namespace Game2048.Game.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            int loadedCount;
            TD_LearningAgent agent = new TD_LearningAgent(0.0025f, out loadedCount);
            Console.WriteLine("Loaded {0} TupleFeatures", loadedCount);
            agent.Training(500000, 1000, ConsoleGameEnvironment.PrintBoard);
            //Game.Library.Game game = new Library.Game();
            //ConsoleGameEnvironment.PrintBoard(game.Board);
            //while (!game.IsEnd)
            //{
            //    ulong movedRawBlocks = game.Move(ConsoleGameEnvironment.GetDirection());
            //    ulong blocksAfterAdded = game.Board.RawBlocks;
            //    ConsoleGameEnvironment.PrintBoard(game.Board);
            //}

            Console.Read();
        }
    }
}
