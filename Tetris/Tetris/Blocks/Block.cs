namespace Tetris.Blocks
{
    public abstract class Block
    {
        private int rotationState;
        private Position offset;

        public Block()
        {
            this.offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        protected abstract Position[][] Tiles { get; }

        protected abstract Position StartOffset { get; }

        public abstract int Id { get; }

        public IEnumerable<Position> TilePositons()
        {
            foreach (var positon in Tiles[this.rotationState]) 
            {
                yield return new Position(positon.Row + this.offset.Row, positon.Column + this.offset.Column);
            }
        }

        public void RotateCW() 
        {
            this.rotationState = (this.rotationState + 1) % this.Tiles.Length;
        }

        public void RotateCCW()
        {
            if (this.rotationState == 0)
            {
                this.rotationState = this.Tiles.Length - 1;
            }
            else 
            {
                this.rotationState--;
            }
        }

        public void Move(int rows, int columns)
        {
            this.offset.Row += rows;
            this.offset.Column += columns;
        }

        public void Reset()
        {
            this.rotationState = 0;
            this.offset.Row = this.StartOffset.Row;
            this.offset.Column = this.StartOffset.Column;
        }
    }
}
