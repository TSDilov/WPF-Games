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
            this.CanHold = true;
        }

        public Block CurrentBlock
        {
            get => this.currentBlock;
            private set 
            {
                this.currentBlock = value;
                this.currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    this.currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        this.currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }

        public BlockQueue BlockQueue { get; }

        public bool GameOver { get; private set; }

        public int Score { get; private set; }

        public Block HeldBlock { get; private set; }

        public bool CanHold { get; private set; }

        public void RotateBlockCW()
        {
            this.CurrentBlock.RotateCW();

            if (!BlockFits()) 
            {
                this.CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW()
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

        public void HoldBlock()
        {
            if (!this.CanHold)
            {
                return;
            }

            if (this.HeldBlock == null)
            {
                this.HeldBlock = this.CurrentBlock;
                this.CurrentBlock = this.BlockQueue.GetAndUpdate();
            }
            else 
            {
                var temp = this.CurrentBlock;
                this.CurrentBlock = this.HeldBlock;
                this.HeldBlock = temp;
            }

            this.CanHold = false;
        }

        public void DropBlock()
        {
            this.CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public int BlockDropDistance()
        {
            var drop = this.GameGrid.Rows;

            foreach (var position in this.CurrentBlock.TilePositons())
            {
                drop = Math.Min(drop, TileDropDistance(position));
            }

            return drop;

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

            this.Score += this.GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                this.GameOver = true;
            }
            else 
            {
                this.CurrentBlock = this.BlockQueue.GetAndUpdate();
                this.CanHold = true;
            }
        }

        private int TileDropDistance(Position position)
        {
            var drop = 0;

            while (this.GameGrid.IsEmpty(position.Row + drop + 1, position.Column)) 
            {
                drop++;
            }

            return drop;
        }
    }
}
