using Game2048.AI.TD_Learning.TupleFeatures;
using Game2048.Game.Library;
using System.Collections.Generic;

namespace Game2048.AI.TD_Learning
{
    public class TupleNetwork
    {
        public List<TupleFeature> featureSet;
        private ulong[] rotatedBoards;

        public TupleNetwork()
        {
            featureSet = new List<TupleFeature>();
            featureSet.Add(new SixTupleFeature(2));
            featureSet.Add(new SixTupleFeature(7));
            featureSet.Add(new SixTupleFeature(3));
            featureSet.Add(new SixTupleFeature(1));
            featureSet.Add(new SixTupleFeature(6));
            featureSet.Add(new SixTupleFeature(5));

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
        public void SetLocalRoatatedBoards(ulong rawBlocks)
        {
            ushort[] rowContents = new ushort[4];
            ushort[] reversedRowContents = new ushort[4];
            ushort[] verticalFillpedRowContents = new ushort[4];
            ushort[] reversedVerticalFlippedRowContents = new ushort[4];

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
            
        }
        public void Load()
        {

        }
    }
}
