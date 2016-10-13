using System;
using Game2048.Game.Library;

namespace Game2048.Game.ConsoleVersion
{
    public static class ConsoleGameEnvironment
    {
        public static void PrintBoard(BitBoard board)
        {
            for (int columnIndex = 0; columnIndex < 4; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < 4; rowIndex++)
                {
                    if (((board.RawBlocks >> (16 * columnIndex + 4 * rowIndex)) & 0xf) > 0)
                        Console.Write("\t{0}", 1 << (int)((board.RawBlocks >> (16 * columnIndex + 4 * rowIndex)) & 0xf));
                    else
                        Console.Write("\t0");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static Direction GetDirection()
        {
            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.W:
                    return Direction.Up;
                case ConsoleKey.S:
                    return Direction.Down;
                case ConsoleKey.A:
                    return Direction.Left;
                case ConsoleKey.D:
                    return Direction.Right;
                default:
                    return Direction.No;
            }
        }
    }
}
