namespace Game2048.AI.TD_Learning
{
    public interface ILearningAI
    {
        Game.Library.Game Train(bool isUsedRawBoard = false, ulong rawBoard = 0);
        Game.Library.Game PlayGame(bool isUsedRawBoard = false, ulong rawBoard = 0);
        void Save();
    }
}
