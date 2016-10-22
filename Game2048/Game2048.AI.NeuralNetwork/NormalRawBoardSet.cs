using Game2048.Game.Library;
using MsgPack.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Game2048.AI.NeuralNetwork
{
    public static class NormalRawBoardSet
    {
        private static HashSet<ulong> uniqueNormalRawBoardSet;
        public static IEnumerable<ulong> NormalRawBoards { get { return uniqueNormalRawBoardSet; } }
        public static int SetSize { get { return uniqueNormalRawBoardSet.Count; } }

        static NormalRawBoardSet()
        {
            uniqueNormalRawBoardSet = new HashSet<ulong>();
            Load();
        }

        public static bool AddRawBoard(ulong rawBoard)
        {
            if (BitBoard.RawEmptyCountTest(rawBoard) > 6)
            {
                return uniqueNormalRawBoardSet.Add(rawBoard);
            }
            else
            {
                return false;
            }
        }
        public static bool Load()
        {
            if (File.Exists("NormalRawBoardSet"))
            {
                var serializer = MessagePackSerializer.Get<HashSet<ulong>>();
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("NormalRawBoardSet")))
                {
                    uniqueNormalRawBoardSet = serializer.Unpack(ms);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Save()
        {
            var serializer = MessagePackSerializer.Get<HashSet<ulong>>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.Pack(memoryStream, uniqueNormalRawBoardSet);
                File.WriteAllBytes("NormalRawBoardSet", memoryStream.ToArray());
            }
        }
    }
}
