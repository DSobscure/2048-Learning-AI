using Game2048.AI.TD_Learning.TupleFeatures;
using Game2048.Game.Library;
using System.Collections.Generic;
using System.IO;

namespace Game2048.AI.TD_Learning
{
    public unsafe class SimpleTupleNetwork : TupleNetwork
    {
        public List<TupleFeature> featureSet;

        public SimpleTupleNetwork(string networkName, int index) : base(networkName, index)
        {
            featureSet = new List<TupleFeature>();
            switch (index)
            {
                case 0:
                    featureSet.Add(new SixTupleFeature(1));
                    featureSet.Add(new SixTupleFeature(101));
                    featureSet.Add(new SixTupleFeature(502));
                    featureSet.Add(new SixTupleFeature(602));
                    break;
                case 1:
                    featureSet.Add(new FourTupleFeature(1));
                    featureSet.Add(new FourTupleFeature(2));
                    featureSet.Add(new FourTupleFeature(3));
                    featureSet.Add(new FourTupleFeature(4));
                    featureSet.Add(new FourTupleFeature(5));
                    break;
                case 2:
                    featureSet.Add(new FourTupleFeature(6));
                    featureSet.Add(new FourTupleFeature(7));
                    featureSet.Add(new FourTupleFeature(8));
                    featureSet.Add(new FourTupleFeature(9));
                    featureSet.Add(new FourTupleFeature(10));
                    break;
                case 3:
                    featureSet.Add(new FiveTupleFeature(1));
                    featureSet.Add(new FiveTupleFeature(2));
                    featureSet.Add(new FiveTupleFeature(3));
                    featureSet.Add(new FiveTupleFeature(4));
                    featureSet.Add(new FiveTupleFeature(5));
                    featureSet.Add(new FiveTupleFeature(6));
                    break;
            }
        }
        public override float GetValue(ulong[] symmetricBoards)
        {
            float sum = 0;
            for (int i = 0; i < featureSet.Count; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    sum += featureSet[i].GetScore(symmetricBoards[j]);
                }
            }
            return sum;
        }
        public override void UpdateValue(ulong[] symmetricBoards, float delta)
        {
            for (int i = 0; i < featureSet.Count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    featureSet[i].UpdateScore(symmetricBoards[j], delta);
                }
            }
        }
        public override void Save()
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
        public override void Load(out int loadedCount)
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

        public unsafe ulong GetMirrorSymmetricRawBlocks(ulong rawBlocks)
        {
            ushort* reversedRowContents = stackalloc ushort[4];

            for (int i = 0; i < 4; i++)
            {
                reversedRowContents[i] = BitBoardOperationSet.ReverseRow(BitBoardOperationSet.GetRow(rawBlocks, i));
            }

            return BitBoardOperationSet.SetRows(reversedRowContents); ;
        }
    }
}
