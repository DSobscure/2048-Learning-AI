using Game2048.AI.TD_Learning.TupleFeatures;
using Game2048.Game.Library;
using System.Collections.Generic;
using System.IO;

namespace Game2048.AI.TD_Learning
{
    public unsafe class TupleNetwork
    {
        public List<TupleFeature> featureSet;
        private ulong[] rotatedBoards;
        public string NetworkName { get; private set; }

        public TupleNetwork(string networkName, int index)
        {
            NetworkName = networkName;
            featureSet = new List<TupleFeature>();
            switch (index)
            {
                case 0:
                    featureSet.Add(new FourTupleFeature(3));
                    featureSet.Add(new FourTupleFeature(7));
                    featureSet.Add(new FiveTupleFeature(3));
                    featureSet.Add(new SixTupleFeature(1));
                    featureSet.Add(new SixTupleFeature(2));
                    featureSet.Add(new SixTupleFeature(3));
                    featureSet.Add(new SixTupleFeature(7));
                    break;
                case 1:
                    //featureSet.Add(new FourTupleFeature(3));
                    //featureSet.Add(new FourTupleFeature(7));
                    featureSet.Add(new FiveTupleFeature(3));
                    break;
            }
            rotatedBoards = new ulong[4];
        }
        public float GetValue(ulong blocks)
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
        public void UpdateValue(ulong blocks, float delta)
        {
            SetLocalRoatatedBoards(blocks);
            featureSet.ForEach(x => x.SetSymmetricBoards(rotatedBoards));
            featureSet.ForEach(x => x.UpdateScore(blocks, delta));
        }
        public unsafe void SetLocalRoatatedBoards(ulong rawBlocks)
        {
            ushort* rowContents = stackalloc ushort[4];
            ushort* reversedRowContents = stackalloc ushort[4];
            ushort* verticalFillpedRowContents = stackalloc ushort[4];
            ushort* reversedVerticalFlippedRowContents = stackalloc ushort[4];

            for (int i = 0; i < 4; i++)
            {
                rowContents[i] = BitBoardOperationSet.GetRow(rawBlocks, i);
                reversedRowContents[i] = BitBoardOperationSet.ReverseRow(rowContents[i]);
                verticalFillpedRowContents[3 - i] = rowContents[i];
                reversedVerticalFlippedRowContents[3 - i] = reversedRowContents[i];
            }

            rotatedBoards[0] = rawBlocks;//origin board
            rotatedBoards[1] = BitBoardOperationSet.SetColumns(reversedRowContents);//clockwise rotate 270
            rotatedBoards[2] = BitBoardOperationSet.SetColumns(verticalFillpedRowContents);//clockwise rotate 90
            rotatedBoards[3] = BitBoardOperationSet.SetRows(reversedVerticalFlippedRowContents);//clockwise rotate 180
        }
        public void Save()
        {
            for (int i = 0; i < featureSet.Count; i++)
            {
                if(featureSet[i] is FourTupleFeature)
                {
                    File.WriteAllBytes(NetworkName + "FourTupleFeature_Index" + i.ToString(), SerializationHelper.Serialize(featureSet[i] as FourTupleFeature));
                }
                else if(featureSet[i] is FiveTupleFeature)
                {
                    File.WriteAllBytes(NetworkName + "FiveTupleFeature_Index" + i.ToString(), SerializationHelper.Serialize(featureSet[i] as FiveTupleFeature));
                }
                else if(featureSet[i] is SixTupleFeature)
                {
                    File.WriteAllBytes(NetworkName + "SixTupleFeature_Index" + i.ToString(), SerializationHelper.Serialize(featureSet[i] as SixTupleFeature));
                }
            }
        }
        public void Load(out int loadedCount)
        {
            loadedCount = featureSet.Count;
            for (int i = 0; i < featureSet.Count; i++)
            {
                if (featureSet[i] is FourTupleFeature && File.Exists(NetworkName + "FourTupleFeature_Index" + i.ToString()))
                {
                    featureSet[i] = SerializationHelper.Deserialize<FourTupleFeature>(File.ReadAllBytes(NetworkName + "FourTupleFeature_Index" + i.ToString()));
                }
                else if (featureSet[i] is FiveTupleFeature && File.Exists(NetworkName + "FiveTupleFeature_Index" + i.ToString()))
                {
                    featureSet[i] = SerializationHelper.Deserialize<FiveTupleFeature>(File.ReadAllBytes(NetworkName + "FiveTupleFeature_Index" + i.ToString()));
                }
                else if (featureSet[i] is SixTupleFeature && File.Exists(NetworkName + "SixTupleFeature_Index" + i.ToString()))
                {
                    featureSet[i] = SerializationHelper.Deserialize<SixTupleFeature>(File.ReadAllBytes(NetworkName + "SixTupleFeature_Index" + i.ToString()));
                }
                else
                {
                    loadedCount--;
                }
            }
        }
    }
}
