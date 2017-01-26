using Game2048.Game.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game2048.AI.TD_Learning
{
    public class LearningAgent
    {
        public static Action<BitBoard> PrintBoardFunction { get; private set; }
        private LearningAI learningAI;

        public LearningAgent(LearningAI learningAI)
        {
            this.learningAI = learningAI;
        }

        public void Training(string logName, TranningMode mode, int startRound, int trainingTimes, int recordSize, Action<BitBoard> printBoardFunction, bool isUsedGivenRawBoards = false, IEnumerable<ulong> givenRawBoards = null)
        {
            PrintBoardFunction = printBoardFunction;

            List<float> scores = new List<float>();
            List<int> steps = new List<int>();
            List<GameRecord> records = new List<GameRecord>();
            try
            {
                int maxScore = 0, minScore = int.MaxValue;
                int maxTile = 0, minTile = 65536;
                int maxCount = 0, minCount = 0;
                int maxStep = 0, minStep = int.MaxValue;
                int win128Count = 0, win256Count = 0, win512Count = 0, win1024Count = 0, win2048Count = 0, win4096Count = 0, win8192Count = 0, win16384Count = 0;
                BitBoard minScoreBoard = null, maxScoreBoard = null, minStepBoard = null, maxStepBoard = null;

                float totalSecond = 0;
                IEnumerator<ulong> rawBoardEnumerator = null;

                float previousAverageScore = 0;
                float previousScoreDeviation = float.MaxValue;
                float previousAverageStep = 0;
                float previousStepDeviation = float.MaxValue;

                if (isUsedGivenRawBoards)
                {
                    rawBoardEnumerator = givenRawBoards.GetEnumerator();
                }
                for (int i = 1; i <= trainingTimes; i++)
                {
                    DateTime startTime = DateTime.Now;
                    Game.Library.Game game = null;
                    if (isUsedGivenRawBoards)
                    {
                        switch (mode)
                        {
                            case TranningMode.Classical:
                                game = learningAI.Train(isUsedGivenRawBoards, rawBoardEnumerator.Current);
                                break;
                            case TranningMode.ScoreTrainedReward:
                                game = learningAI.RewardTrain(true, previousAverageScore, previousScoreDeviation, isUsedGivenRawBoards, rawBoardEnumerator.Current);
                                break;
                            case TranningMode.StepTrainedReward:
                                game = learningAI.RewardTrain(false, previousAverageStep, previousStepDeviation, isUsedGivenRawBoards, rawBoardEnumerator.Current);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        if (!rawBoardEnumerator.MoveNext())
                        {
                            rawBoardEnumerator.Reset();
                            Console.WriteLine("Reset");
                        }
                    }
                    else
                    {
                        switch (mode)
                        {
                            case TranningMode.Classical:
                                game = learningAI.Train();
                                break;
                            case TranningMode.ScoreTrainedReward:
                                game = learningAI.RewardTrain(true, previousAverageScore, previousScoreDeviation);
                                break;
                            case TranningMode.StepTrainedReward:
                                game = learningAI.RewardTrain(false, previousAverageStep, previousStepDeviation);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    totalSecond += Convert.ToSingle((DateTime.Now - startTime).TotalSeconds);

                    scores.Add(game.Score);
                    steps.Add(game.Step);

                    win128Count += (game.Board.MaxTile >= 128) ? 1 : 0;
                    win256Count += (game.Board.MaxTile >= 256) ? 1 : 0;
                    win512Count += (game.Board.MaxTile >= 512) ? 1 : 0;
                    win1024Count += (game.Board.MaxTile >= 1024) ? 1 : 0;
                    win2048Count += (game.Board.MaxTile >= 2048) ? 1 : 0;
                    win4096Count += (game.Board.MaxTile >= 4096) ? 1 : 0;
                    win8192Count += (game.Board.MaxTile >= 8192) ? 1 : 0;
                    win16384Count += (game.Board.MaxTile >= 16384) ? 1 : 0;

                    if (game.Score > maxScore)
                    {
                        maxScore = game.Score;
                        maxScoreBoard = new BitBoard(game.Board.RawBlocks);
                    }
                    if (game.Score < minScore)
                    {
                        minScore = game.Score;
                        minScoreBoard = new BitBoard(game.Board.RawBlocks);
                    }
                    if (game.Step > maxStep)
                    {
                        maxStep = game.Step;
                        maxStepBoard = new BitBoard(game.Board.RawBlocks);
                    }
                    if (game.Step < minStep)
                    {
                        minStep = game.Step;
                        minStepBoard = new BitBoard(game.Board.RawBlocks);
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
                        float totalStep = steps.Sum();

                        float scoreDeviation = (float)Math.Sqrt(scores.Sum(x => Math.Pow(x - totalScore / recordSize, 2)) / recordSize);
                        float stepDeviation = (float)Math.Sqrt(steps.Sum(x => Math.Pow(x - totalStep / recordSize, 2)) / recordSize);

                        previousAverageScore = totalScore / recordSize;
                        previousScoreDeviation = scoreDeviation;

                        previousAverageStep = totalStep / recordSize;
                        previousStepDeviation = stepDeviation;

                        records.Add(new GameRecord
                        {
                            startRound = startRound,
                            roundSize = recordSize,
                            round = i,
                            averageScore = previousAverageScore,
                            averageStep = previousAverageStep,
                            maxScore = maxScore,
                            minScore = minScore,
                            maxStep = maxStep,
                            minStep = minStep,
                            winRate128 = win128Count / (float)recordSize,
                            winRate256 = win256Count / (float)recordSize,
                            winRate512 = win512Count / (float)recordSize,
                            winRate1024 = win1024Count / (float)recordSize,
                            winRate2048 = win2048Count / (float)recordSize,
                            winRate4096 = win4096Count / (float)recordSize,
                            winRate8192 = win8192Count / (float)recordSize,
                            winRate16384 = win16384Count / (float)recordSize,
                            scoreDeviation = scoreDeviation,
                            stepDeviation = stepDeviation,
                            deltaTime = totalSecond,
                            averageSpeed = totalStep / totalSecond,
                            rawMaxScoreBoard = maxScoreBoard.RawBlocks,
                            rawMinScoreBoard = minScoreBoard.RawBlocks,
                            rawMaxStepBoard = maxStepBoard.RawBlocks,
                            rawMinStepBoard = minStepBoard.RawBlocks
                        });
                        Console.WriteLine($"Round: {i} AvgScore: {previousAverageScore} - Deviation: {scoreDeviation}, AverageStep: {previousAverageStep} - Deviation: {stepDeviation}");
                        Console.WriteLine($"Max Score: {maxScore}, Min Score: {minScore}");
                        Console.WriteLine($"Max Steps: {maxStep}, Min Steps: {minStep}");
                        Console.WriteLine($"128WinRate: {win128Count / (float)recordSize}");
                        Console.WriteLine($"256WinRate: {win256Count / (float)recordSize}");
                        Console.WriteLine($"512WinRate: {win512Count / (float)recordSize}");
                        Console.WriteLine($"1024WinRate: {win1024Count / (float)recordSize}");
                        Console.WriteLine($"2048WinRate: {win2048Count / (float)recordSize}");
                        Console.WriteLine($"4096WinRate: {win4096Count / (float)recordSize}");
                        Console.WriteLine($"8192WinRate: {win8192Count / (float)recordSize}");
                        Console.WriteLine($"16384WinRate: {win16384Count / (float)recordSize}");
                        Console.WriteLine($"Max Tile: {maxTile} #{maxCount}, Min Tile: {minTile} #{minCount}");
                        Console.WriteLine("Delta Time: {0} seconds", totalSecond);
                        Console.WriteLine("Average Speed: {0}moves/sec", totalStep / totalSecond);
                        Console.WriteLine();

                        printBoardFunction(minScoreBoard);
                        printBoardFunction(maxScoreBoard);

                        win128Count = 0;
                        win256Count = 0;
                        win512Count = 0;
                        win1024Count = 0;
                        win2048Count = 0;
                        win4096Count = 0;
                        win8192Count = 0;
                        win16384Count = 0;
                        maxTile = 0;
                        minTile = 65536;
                        maxCount = 0;
                        minCount = 0;
                        maxStep = 0;
                        minStep = int.MaxValue;
                        totalSecond = 0;
                        maxScore = int.MinValue;
                        minScore = int.MaxValue;
                        scores.Clear();
                        steps.Clear();
                        minScoreBoard = null;
                        maxScoreBoard = null;
                        minStepBoard = null;
                        maxStepBoard = null;
                        if (i % 20000 == 0)
                        {
                            learningAI.Save();
                            File.WriteAllBytes(logName, SerializationHelper.Serialize(records));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                File.WriteAllBytes(logName, SerializationHelper.Serialize(records));
            }           
        }
    }
}
