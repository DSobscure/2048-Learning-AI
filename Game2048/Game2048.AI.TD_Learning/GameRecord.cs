using MsgPack.Serialization;

namespace Game2048.AI.TD_Learning
{
    public class GameRecord
    {
        [MessagePackMember(id:0)]
        public int startRound;
        [MessagePackMember(id: 1)]
        public int roundSize;
        [MessagePackMember(id: 2)]
        public int round;
        [MessagePackMember(id: 3)]
        public float averageScore;
        [MessagePackMember(id: 4)]
        public float averageStep;
        [MessagePackMember(id: 5)]
        public int maxScore;
        [MessagePackMember(id: 6)]
        public int minScore;
        [MessagePackMember(id: 7)]
        public int maxStep;
        [MessagePackMember(id: 8)]
        public int minStep;
        [MessagePackMember(id: 9)]
        public float winRate128;
        [MessagePackMember(id: 10)]
        public float winRate256;
        [MessagePackMember(id: 11)]
        public float winRate512;
        [MessagePackMember(id: 12)]
        public float winRate1024;
        [MessagePackMember(id: 13)]
        public float winRate2048;
        [MessagePackMember(id: 14)]
        public float winRate4096;
        [MessagePackMember(id: 15)]
        public float winRate8192;
        [MessagePackMember(id: 16)]
        public float winRate16384;
        [MessagePackMember(id: 17)]
        public float scoreDeviation;
        [MessagePackMember(id: 18)]
        public float stepDeviation;
        [MessagePackMember(id: 19)]
        public float deltaTime;
        [MessagePackMember(id: 20)]
        public float averageSpeed;
        [MessagePackMember(id: 21)]
        public ulong rawMaxScoreBoard;
        [MessagePackMember(id: 22)]
        public ulong rawMinScoreBoard;
        [MessagePackMember(id: 23)]
        public ulong rawMaxStepBoard;
        [MessagePackMember(id: 24)]
        public ulong rawMinStepBoard;
    }
}
