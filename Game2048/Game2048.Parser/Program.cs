using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Game2048.Game.Library;
using Game2048.AI.TD_Learning;
using Game2048.AI.NeuralNetwork;

namespace Game2048.Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input file name");
            string fileName = Console.ReadLine();
            List<GameRecord> records = SerializationHelper.Deserialize<List<GameRecord>>(File.ReadAllBytes(fileName));
            Console.WriteLine("Total {0}", records.Count);
            Console.WriteLine("Input Start Record Index");
            int startIndex = int.Parse(Console.ReadLine());
            Console.WriteLine("Input End Record Index");
            int endIndex = int.Parse(Console.ReadLine());
            using (TextWriter logger = File.CreateText("Records"))
            {
                for (int i = startIndex; i < records.Count && i <= endIndex; i++)
                {
                    logger.WriteLine("{0}", records[i].averageScore);
                    //logger.WriteLine("{0}\t{1}\t{2}\t{3}", records[i].averageScore, records[i].maxScore, records[i].minScore, records[i].scoreDeviation);
                    //Console.WriteLine($"Round: {records[i].round} AvgScore: {records[i].averageScore} - Deviation: {records[i].scoreDeviation}, AverageStep: {records[i].averageStep} - Deviation: {records[i].stepDeviation}");
                    //Console.WriteLine($"Max Score: {records[i].maxScore}, Min Score: {records[i].minScore}");
                    //Console.WriteLine($"Max Steps: {records[i].maxStep}, Min Steps: {records[i].minStep}");
                    //Console.WriteLine($"128WinRate: {records[i].winRate128}");
                    //Console.WriteLine($"256WinRate: {records[i].winRate256}");
                    //Console.WriteLine($"512WinRate: {records[i].winRate512}");
                    //Console.WriteLine($"1024WinRate: {records[i].winRate1024}");
                    //Console.WriteLine($"2048WinRate: {records[i].winRate2048}");
                    //Console.WriteLine($"4096WinRate: {records[i].winRate4096}");
                    //Console.WriteLine($"8192WinRate: {records[i].winRate8192}");
                    //Console.WriteLine($"16384WinRate: {records[i].winRate16384}");
                    //Console.WriteLine("Delta Time: {0} seconds", records[i].deltaTime);
                    //Console.WriteLine("Average Speed: {0}moves/sec", records[i].averageSpeed);
                    //Console.WriteLine();
                }
            }
            Console.WriteLine("End");
            //Console.ReadLine();
        }
    }
}
