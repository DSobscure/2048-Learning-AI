using Game2048.AI.NeuralNetwork;
using Game2048.Game.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game2048.AI.TD_Learning
{
    public class TD_LearningAgent
    {
        private TD_LearningAI ai;

        public TD_LearningAgent(float learningRate, out int loadedCount)
        {
            ai = new TD_LearningAI(learningRate, out loadedCount);
        }

        public void Training(int trainingTimes, int recordSize, Action<BitBoard> printBoardFunction, bool isUsedGivenRawBoards = false, IEnumerable<ulong> givenRawBoards = null)
        {
            List<float> scores = new List<float>();
            int maxScore = int.MinValue;
            int minScore = int.MaxValue;
            int maxTile = int.MinValue;
            int minTile = int.MaxValue;
            int maxCount = 0;
            int minCount = 0;
            int maxStep = int.MinValue;
            int minStep = int.MaxValue;
            int winCount = 0;
            BitBoard minBoard = null;
            BitBoard maxBoard = null;
            float totalSecond = 0;
            int totalSteps = 0;
            IEnumerator<ulong> rawBoardEnumerator = null;
            int solvedEndgameCounter = 0;

            if (isUsedGivenRawBoards)
            {
                rawBoardEnumerator = givenRawBoards.GetEnumerator();
            }
            for (int i = 1; i <= trainingTimes; i++)
            {
                DateTime startTime = DateTime.Now;
                Game.Library.Game game;
                if (isUsedGivenRawBoards)
                {
                    game = ai.Train(isUsedGivenRawBoards, rawBoardEnumerator.Current);
                    if(!rawBoardEnumerator.MoveNext())
                    {
                        rawBoardEnumerator.Reset();
                        Console.WriteLine("Reset");
                    }
                    if(!game.IsEnd)
                    {
                        solvedEndgameCounter++;
                    }
                }
                else
                {
                    game = ai.Train();
                }
                totalSecond += Convert.ToSingle((DateTime.Now - startTime).TotalSeconds);
                totalSteps += game.Step;
                scores.Add(game.Score);

                if (game.Board.MaxTile >= 2048)
                {
                    winCount++;
                }

                if (game.Score > maxScore)
                {
                    maxScore = game.Score;
                    maxBoard = new BitBoard(game.Board.RawBlocks);
                }

                if (game.Score < minScore)
                {
                    minScore = game.Score;
                    minBoard = new BitBoard(game.Board.RawBlocks);
                }

                if (game.Step > maxStep)
                {
                    maxStep = game.Step;
                }

                if (game.Step < minStep)
                {
                    minStep = game.Step;
                }

                if (game.Board.MaxTile > maxTile)
                {
                    maxTile = game.Board.MaxTile;
                    maxCount = 1;
                }
                else if (game.Board.MaxTile == maxTile)
                {
                    maxCount++;
                }

                if (game.Board.MaxTile < minTile)
                {
                    minTile = game.Board.MaxTile;
                    minCount = 1;
                }
                else if (game.Board.MaxTile == minTile)
                {
                    minCount++;
                }

                if (i % recordSize == 0)
                {
                    float totalScore = scores.Sum();
                    double deviation = Math.Sqrt(scores.Sum(x => Math.Pow(x - totalScore / recordSize, 2)) / recordSize);

                    Console.WriteLine("Round: {0} AvgScore: {1}", i, totalScore / recordSize);
                    Console.WriteLine("Endgame Solved Rate: {0}", solvedEndgameCounter / (double)recordSize);
                    Console.WriteLine("Max Score: {0}", scores.Max());
                    Console.WriteLine("Min Score: {0}", scores.Min());
                    Console.WriteLine("Max Steps: {0}", maxStep);
                    Console.WriteLine("Min Steps: {0}", minStep);
                    Console.WriteLine("Win Rate: {0}", winCount * 1.0 / recordSize);
                    Console.WriteLine("Max Tile: {0} #{1}", maxTile, maxCount);
                    Console.WriteLine("Min Tile: {0} #{1}", minTile, minCount);
                    Console.WriteLine("標準差: {0}", deviation);
                    Console.WriteLine("Delta Time: {0} seconds", totalSecond);
                    Console.WriteLine("Average Speed: {0}moves/sec", totalSteps / totalSecond);

                    Console.WriteLine();
                    printBoardFunction(minBoard);
                    printBoardFunction(maxBoard);

                    winCount = 0;
                    maxTile = 0;
                    minTile = 65536;
                    maxCount = 0;
                    minCount = 0;
                    maxStep = 0;
                    minStep = 1000000;
                    totalSecond = 0;
                    totalSteps = 0;
                    maxScore = int.MinValue;
                    minScore = int.MaxValue;
                    scores.Clear();
                    minBoard = null;
                    maxBoard = null;
                    solvedEndgameCounter = 0;

                    ai.SaveTupleNetwork();
                }
            }
            //EndgameRawBoardSet.Save();
            //NormalRawBoardSet.Save();
        }
    }
}
