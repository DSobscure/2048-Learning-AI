namespace Game2048.Game.Library
{
    public class Game
    {
        public BitBoard Board { get; private set; }
        public int Score { get; private set; }
        public int Step { get; private set; }
        public bool IsEnd
        {
            get
            {
                return !Board.CanMove;
            }
        }
        public Game()
        {
            Board = new BitBoard(0);
            Board.Initial();
            Score = 0;
            Step = 0;
        }
        public ulong Move(Direction direction)
        {
            ulong movedRawBlocks= Board.RawBlocks;
            if (Board.MoveCheck(direction))
            {
                int reward;
                Board = Board.Move(direction, out reward);
                Score += reward;
                Step++;
                movedRawBlocks = Board.RawBlocks;
                Board.InsertNewTile();
            }
            return movedRawBlocks;
        }
    }
}
