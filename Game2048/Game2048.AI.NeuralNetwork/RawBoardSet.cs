using MsgPack.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Game2048.AI.NeuralNetwork
{
    public class RawBoardSet
    {
        private HashSet<ulong> uniqueRawBoardSet;
        public IEnumerable<ulong> RawBoards { get { return uniqueRawBoardSet; } }
        public int SetSize { get { return uniqueRawBoardSet.Count; } }
        public string SetName { get; private set; }

        public RawBoardSet(string setName)
        {
            uniqueRawBoardSet = new HashSet<ulong>();
            SetName = setName;
            Load();
        }

        public bool AddRawBoard(ulong rawBoard)
        {
            return uniqueRawBoardSet.Add(rawBoard);
        }
        public bool Contains(ulong rawBoard)
        {
            return uniqueRawBoardSet.Contains(rawBoard);
        }
        public bool Load()
        {
            if (File.Exists(SetName))
            {
                var serializer = MessagePackSerializer.Get<HashSet<ulong>>();
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(SetName)))
                {
                    uniqueRawBoardSet = serializer.Unpack(ms);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Save()
        {
            var serializer = MessagePackSerializer.Get<HashSet<ulong>>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.Pack(memoryStream, uniqueRawBoardSet);
                File.WriteAllBytes(SetName, memoryStream.ToArray());
            }
        }
    }
}
