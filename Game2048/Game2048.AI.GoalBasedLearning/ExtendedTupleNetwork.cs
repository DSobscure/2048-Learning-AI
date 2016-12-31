using Game2048.AI.GoalBasedLearning.ExtendedTupleFeatures;
using Game2048.Game.Library;
using System.Collections.Generic;
using System.IO;
using System;

namespace Game2048.AI.GoalBasedLearning
{
    public class ExtendedTupleNetwork
    {
        public List<ExtendedTupleFeature> featureSet;
        private ExtendedBitBoard[] rotatedBoards;
        public string NetworkName { get; private set; }

        public ExtendedTupleNetwork(string networkName, int index)
        {
            NetworkName = networkName;
            featureSet = new List<ExtendedTupleFeature>();
            switch (index)
            {
                case 1:
                    featureSet.Add(new ExtendedFourTupleFeature(3));
                    featureSet.Add(new ExtendedFourTupleFeature(7));
                    featureSet.Add(new ExtendedFiveTupleFeature(3));
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            rotatedBoards = new ExtendedBitBoard[4];
        }
        public float GetValue(ExtendedBitBoard blocks)
        {
            SetLocalRoatatedBoards(blocks);
            featureSet.ForEach(x => x.SetSymmetricBoards(rotatedBoards));
            float sum = 0;
            for (int i = 0; i < featureSet.Count; i++)
            {
                sum += featureSet[i].GetScore(blocks);
            }
            return sum;
        }
        public void UpdateValue(ExtendedBitBoard blocks, float delta)
        {
            SetLocalRoatatedBoards(blocks);
            featureSet.ForEach(x => x.SetSymmetricBoards(rotatedBoards));
            featureSet.ForEach(x => x.UpdateScore(blocks, delta));
        }
        public unsafe void SetLocalRoatatedBoards(ExtendedBitBoard rawBlocks)
        {
            uint* rowContents = stackalloc uint[4];
            uint* reversedRowContents = stackalloc uint[4];
            uint* verticalFillpedRowContents = stackalloc uint[4];
            uint* reversedVerticalFlippedRowContents = stackalloc uint[4];

            for (int i = 0; i < 4; i++)
            {
                rowContents[i] = ExtendedBitBoardOperationSet.GetRow(rawBlocks, i);
                reversedRowContents[i] = ExtendedBitBoardOperationSet.ReverseRow(rowContents[i]);
                verticalFillpedRowContents[3 - i] = rowContents[i];
                reversedVerticalFlippedRowContents[3 - i] = reversedRowContents[i];
            }

            rotatedBoards[0] = rawBlocks;//origin board
            rotatedBoards[1] = ExtendedBitBoardOperationSet.SetColumns(reversedRowContents);//clockwise rotate 270
            rotatedBoards[2] = ExtendedBitBoardOperationSet.SetColumns(verticalFillpedRowContents);//clockwise rotate 90
            rotatedBoards[3] = ExtendedBitBoardOperationSet.SetRows(reversedVerticalFlippedRowContents);//clockwise rotate 180
        }
        public void Save()
        {
            for (int i = 0; i < featureSet.Count; i++)
            {
                if (featureSet[i] is ExtendedFourTupleFeature)
                {
                    File.WriteAllBytes(NetworkName + "ExtendedFourTupleFeature_Index" + i.ToString(), SerializationHelper.Serialize(featureSet[i] as ExtendedFourTupleFeature));
                }
                else if (featureSet[i] is ExtendedFiveTupleFeature)
                {
                    File.WriteAllBytes(NetworkName + "ExtendedFiveTupleFeature_Index" + i.ToString(), SerializationHelper.Serialize(featureSet[i] as ExtendedFiveTupleFeature));
                }
            }
        }
        public void Load(out int loadedCount)
        {
            loadedCount = featureSet.Count;
            for (int i = 0; i < featureSet.Count; i++)
            {
                if (featureSet[i] is ExtendedFourTupleFeature && File.Exists(NetworkName + "ExtendedFourTupleFeature_Index" + i.ToString()))
                {
                    featureSet[i] = SerializationHelper.Deserialize<ExtendedFourTupleFeature>(File.ReadAllBytes(NetworkName + "ExtendedFourTupleFeature_Index" + i.ToString()));
                }
                else if (featureSet[i] is ExtendedFiveTupleFeature && File.Exists(NetworkName + "ExtendedFiveTupleFeature_Index" + i.ToString()))
                {
                    featureSet[i] = SerializationHelper.Deserialize<ExtendedFiveTupleFeature>(File.ReadAllBytes(NetworkName + "ExtendedFiveTupleFeature_Index" + i.ToString()));
                }
                else
                {
                    loadedCount--;
                }
            }
        }
    }
}
