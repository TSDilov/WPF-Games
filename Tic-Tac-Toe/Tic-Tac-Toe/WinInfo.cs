using Tic_Tac_Toe.Enums;

namespace Tic_Tac_Toe
{
    public class WinInfo
    {
        public WinInfo(WinType type, int number)
        {
            this.Type = type;
            this.Number = number;
        }

        public WinInfo(WinType type)
        {
            this.Type = type;
        }

        public WinType Type { get; set; }

        public int Number { get; set; }
    }
}
