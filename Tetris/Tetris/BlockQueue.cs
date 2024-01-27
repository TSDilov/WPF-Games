using Tetris.Blocks;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock(),
        };

        private readonly Random random = new Random();

        public BlockQueue()
        {
            this.NextBlock = RandomBlock();
        }

        public Block NextBlock { get; private set; }

        public Block GetAndUpdate()
        {
            var block = this.NextBlock;
            do
            {
                this.NextBlock = RandomBlock();
            } while (block.Id == this.NextBlock.Id);

            return block;
        }

        private Block RandomBlock()
        {
            return this.blocks[random.Next(this.blocks.Length)];
        }
    }
}
