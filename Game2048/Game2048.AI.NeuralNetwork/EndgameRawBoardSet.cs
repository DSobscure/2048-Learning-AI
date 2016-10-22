using Game2048.Game.Library;
using MsgPack.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Game2048.AI.NeuralNetwork
{
    public static class EndgameRawBoardSet
    {
        private static HashSet<ulong> uniqueEndgameRawBoardSet;
        public static IEnumerable<ulong> EndGameRawBoards { get { return uniqueEndgameRawBoardSet; } }
        public static int SetSize { get { return uniqueEndgameRawBoardSet.Count; } }

        static EndgameRawBoardSet()
        {
            uniqueEndgameRawBoardSet = new HashSet<ulong>();
            Load();
        }

        public static bool AddRawBoard(ulong rawBoard)
        {
            if(BitBoard.RawEmptyCountTest(rawBoard) < 5)
            {
                return uniqueEndgameRawBoardSet.Add(rawBoard);
            }
            else
            {
                return false;
            }
        }
        public static bool Load()
        {
            if (File.Exists("EndgameRawBoardSet"))
            {
                var serializer = MessagePackSerializer.Get<HashSet<ulong>>();
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes("EndgameRawBoardSet")))
                {
                    uniqueEndgameRawBoardSet = serializer.Unpack(ms);
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
                serializer.Pack(memoryStream, uniqueEndgameRawBoardSet);
                File.WriteAllBytes("EndgameRawBoardSet", memoryStream.ToArray());
            }
        }
    }
}
