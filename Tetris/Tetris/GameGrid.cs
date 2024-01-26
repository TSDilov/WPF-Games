namespace Tetris
{
    public class GameGrid
    {
        private readonly int[,] grid;

        public GameGrid(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            grid = new int[rows, columns];
        }

        public int Rows { get; }

        public int Columns { get; }

        public int this[int r, int c]
        {
            get => this.grid[r, c];
            set => this.grid[r, c] = value;
        }

        public bool IsInside(int r, int c)
        {
            return r >= 0 && r < this.Rows && c >= 0 && c < this.Columns;
        }

        public bool IsEmpty(int r, int c)
        {
            return IsInside(r, c) && grid[r, c] == 0;
        }

        public bool IsRowFull(int r)
        {
            for (int c = 0; c < this.Columns; c++)
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < this.Columns; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public int ClearFullRows()
        {
            var cleared = 0;
            for (int r = this.Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }

        private void ClearRow(int r)
        {
            for (int c = 0; c < this.Columns; c++)
            {
                grid[r, c] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < this.Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }
    }
}
