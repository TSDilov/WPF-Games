using Tic_Tac_Toe.Enums;

namespace Tic_Tac_Toe
{
    public class GameState
    {
        public event Action<int, int> MoveMade;
        public event Action<GameResult> GameEnded;
        public event Action GameRestarted;

        public GameState()
        {
            this.GameGrid = new Player[3, 3];
            this.CurrentPlayer = Player.X;
            this.TurnsPassed = 0;
            this.GameOver = false;
        }

        public Player[,] GameGrid { get; private set; }

        public Player CurrentPlayer { get; private set; }

        public int TurnsPassed { get; private set; }

        public bool GameOver { get; set; }

        public void MakeMove(int r, int c)
        {
            if (!CanMakeMove(r, c))
            {
                return;
            }

            this.GameGrid[r, c] = this.CurrentPlayer;
            this.TurnsPassed++;
            
            if (DidMoveEndGame(r, c, out GameResult gameResult))
            {
                this.GameOver = true;
                MoveMade?.Invoke(r, c);
                GameEnded?.Invoke(gameResult);
            }
            else
            {
                SwitchPlayer();
                MoveMade?.Invoke(r, c);
            }
        }

        public void Reset()
        {
            this.GameGrid = new Player[3, 3];
            this.CurrentPlayer = Player.X;
            this.TurnsPassed = 0;
            this.GameOver = false;
            GameRestarted?.Invoke();
        }

        private bool CanMakeMove(int row, int col)
        {
            return !this.GameOver && this.GameGrid[row, col] == Player.None;
        }

        private bool IsGridFull()
        {
            return this.TurnsPassed == 9;
        }

        private void SwitchPlayer()
        {
            this.CurrentPlayer = this.CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        private bool AreSquaresMarked((int, int)[] squares, Player player)
        {
            foreach ((int r, int c) in squares) 
            {
                if (this.GameGrid[r,c] != player)
                {
                    return false;
                }
            }

            return true;
        }

        private bool DidMoveWin(int r, int c, out WinInfo winInfo)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { (0, c), (1, c), (2, c) };
            (int, int)[] mainDiag = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antiDiag = new[] { (0, 2), (1, 1), (2, 0) };

            if (AreSquaresMarked(row, this.CurrentPlayer))
            {
                winInfo = new WinInfo(WinType.Row, r);
                return true;
            }

            if (AreSquaresMarked(col, this.CurrentPlayer))
            {
                winInfo = new WinInfo(WinType.Column, c);
                return true;
            }

            if (AreSquaresMarked(mainDiag, this.CurrentPlayer))
            {
                winInfo = new WinInfo(WinType.MainDiagonal);
                return true;
            }

            if (AreSquaresMarked(antiDiag, this.CurrentPlayer))
            {
                winInfo = new WinInfo(WinType.AntiDiagonal);
                return true;
            }

            winInfo = null;
            return false;
        }

        private bool DidMoveEndGame(int r, int c, out GameResult gameResult)
        {
            if (DidMoveWin(r, c, out WinInfo winInfo)) 
            {
                gameResult = new GameResult(this.CurrentPlayer, winInfo);
                return true;
            }

            if (IsGridFull())
            {
                gameResult = new GameResult(Player.None);
                return true;
            }

            gameResult = null;
            return false;
        }
    }
}
