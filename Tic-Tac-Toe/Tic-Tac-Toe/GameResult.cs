using Tic_Tac_Toe.Enums;

namespace Tic_Tac_Toe
{
    public class GameResult
    {
        public GameResult(Player winner, WinInfo winInfo)
        {
            this.Winner = winner;
            this.WinInfo = winInfo;
        }

        public GameResult(Player winner)
        {
            this.Winner = winner;
        }

        public Player Winner { get; set; }

        public WinInfo WinInfo { get; set; }
    }
}
