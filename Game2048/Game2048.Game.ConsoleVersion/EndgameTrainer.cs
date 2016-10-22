using Game2048.AI.NeuralNetwork;
using System.Collections.Generic;

namespace Game2048.Game.ConsoleVersion
{
    class EndgameTrainer
    {
        private EndgameClassifier endgameClassifier;

        public EndgameTrainer()
        {
            endgameClassifier = new EndgameClassifier("EndgameClassifier_LearningRate0.01_Layers64_64_64_16_4");

            IEnumerator<ulong> endgameBoardEnumerator = EndgameRawBoardSet.EndGameRawBoards.GetEnumerator();
            IEnumerator<ulong> normalBoardEnumerator = NormalRawBoardSet.NormalRawBoards.GetEnumerator();

            int endgameErrorCounter = 0;
            int normalErrorCounter = 0;
            for (int i = 1; i <= 10000000; i++)
            {
                double error;
                endgameClassifier.TrainDiscreteEndgameBoards(endgameBoardEnumerator.Current, true, out error);
                if (error >= 0.5)
                {
                    endgameErrorCounter++;
                }
                endgameClassifier.TrainDiscreteEndgameBoards(normalBoardEnumerator.Current, false, out error);
                if (error >= 0.5)
                {
                    normalErrorCounter++;
                }
                if(!endgameBoardEnumerator.MoveNext())
                {
                    endgameBoardEnumerator.Reset();
                }
                if (!normalBoardEnumerator.MoveNext())
                {
                    normalBoardEnumerator.Reset();
                }
                if(i % 100000 == 0)
                {
                    System.Console.WriteLine("endgame error rate: {0}", endgameErrorCounter / 100000.0);
                    System.Console.WriteLine("normal error rate: {0}", normalErrorCounter / 100000.0);
                    endgameErrorCounter = 0;
                    normalErrorCounter = 0;
                }
            }
            endgameClassifier.SaveClassifier();
        }
    }
}
