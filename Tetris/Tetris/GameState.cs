using Tetris.Blocks;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;

        public GameState()
        {
            this.GameGrid = new GameGrid(22, 10);
            this.BlockQueue = new BlockQueue();
            this.CurrentBlock = this.BlockQueue.GetAndUpdate();
        }

        public Block CurrentBlock
        {
            get => this.currentBlock;
            private set 
            {
                this.currentBlock = value;
                this.currentBlock.Reset();
            }
        }

        public GameGrid GameGrid { get; }

        public BlockQueue BlockQueue { get; }

        public bool GameOver { get; private set; }

        public void RotateBlockCW()
        {
            this.CurrentBlock.RotateCW();

            if (!BlockFits()) 
            {
                this.CurrentBlock.RotateCCW();
            }
        }

        public void RotateCCW()
        {
            this.CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                this.CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        {
            this.CurrentBlock.Move(0, -1);

            if (!BlockFits()) 
            {
                this.CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            this.CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                this.CurrentBlock.Move(0, -1);
            }
        }

        public void MoveBlockDown()
        {
            this.CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                this.CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private bool BlockFits()
        { 
            foreach (var position in this.CurrentBlock.TilePositons()) 
            {
                if (!this.GameGrid.IsEmpty(position.Row, position.Column))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsGameOver()
        {
            return !(this.GameGrid.IsRowEmpty(0) && this.GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (var position in this.CurrentBlock.TilePositons())
            {
                this.GameGrid[position.Row, position.Column] = this.CurrentBlock.Id;
            }

            this.GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                this.GameOver = true;
            }
            else 
            {
                this.CurrentBlock = this.BlockQueue.GetAndUpdate();
            }
        }
    }
}
