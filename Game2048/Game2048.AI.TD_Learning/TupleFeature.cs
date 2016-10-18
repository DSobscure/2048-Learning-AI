using Game2048.Game.Library;
using MsgPack.Serialization;
using System;
using System.IO;

namespace Game2048.AI.TD_Learning
{
    public abstract class TupleFeature
    {
        public static T Deserialize<T>(byte[] data)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return serializer.Unpack(ms);
            }
        }

        public static byte[] Serialize<T>(T data)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.Pack(memoryStream, data);
                return memoryStream.ToArray();
            }
        }
        [MessagePackMember(id:0, Name = "tuples")]
        protected float[] tuples;
        public virtual void UpdateScore(ulong rawBlocks, float delta)
        {
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                tuples[index] += delta;
                if (symmetricIndex != index)
                    tuples[symmetricIndex] += delta;
            }
        }
        public virtual float GetScore(ulong blocks)
        {
            float sum = 0;
            for (int i = 0; i < 4; i++)
            {
                int index = GetIndex(rotateBoards[i]);
                int symmetricIndex = GetIndex(GetMirrorSymmetricRawBlocks(rotateBoards[i]));

                sum += tuples[index];
                if (symmetricIndex != index)
                    sum += tuples[symmetricIndex];
            }
            return sum;
        }
        public abstract int GetIndex(ulong blocks);

        public ulong[] rotateBoards;
        [MessagePackDeserializationConstructor]
        protected TupleFeature(float[] tuples)
        {
            this.tuples = tuples;
        }
        protected TupleFeature(int tupleNumber)
        {
            tuples = new float[(int)Math.Pow(16, tupleNumber)];
        }
        
        public void SetSymmetricBoards(ulong[] rotateSymmetry)
        {
            rotateBoards = rotateSymmetry;
        }
        public ulong GetMirrorSymmetricRawBlocks(ulong rawBlocks)
        {
            ushort[] reversedRowContents = new ushort[4];

            for (int i = 0; i < 4; i++)
            {
                reversedRowContents[i] = BitBoardOperationSet.ReverseRow(BitBoardOperationSet.GetRow(rawBlocks, i));
            }

            return BitBoardOperationSet.SetRows(reversedRowContents); ;
        }
    }
}
