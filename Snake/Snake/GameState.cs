namespace Snake
{
    public class GameState
    {
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Grid = new GridValue[rows, columns];
            this.Direction = Direction.Right;
            AddSnake();
            AddFood();
        }

        public int Rows { get; }

        public int Columns { get; }

        public GridValue[,] Grid { get; }

        public Direction Direction { get; private set; }

        public int Score { get; private set; }

        public bool GameOver { get; private set; }

        public Position HeadPosition()
        {
            return this.snakePositions.First.Value;
        }

        public Position TailPosition()
        {
            return this.snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return this.snakePositions;
        }

        public void ChangeDirection(Direction direction)
        {
            if (CanChangeDirection(direction))
            {
                this.dirChanges.AddLast(direction);
            }
        }

        public void Move()
        {
            if (this.dirChanges.Count > 0)
            {
                this.Direction = dirChanges.First.Value;
                this.dirChanges.RemoveLast();
            }

            var newHeadPosition = HeadPosition().Translate(this.Direction);
            var hit = WillHit(newHeadPosition);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                this.GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPosition);
            }
            else if (hit == GridValue.Food) 
            {
                AddHead(newHeadPosition);
                this.Score++;
                AddFood();
            }
        }

        private void AddSnake()
        {
            var midRow = this.Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                this.Grid[midRow, c] = GridValue.Snake;
                this.snakePositions.AddFirst(new Position(midRow, c));
            }
        }

        private Direction GetLastDirection()
        {
            if (this.dirChanges.Count == 0)
            {
                return this.Direction;
            }

            return this.dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (this.dirChanges.Count == 2)
            {
                return false;
            }

            var lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Columns; c++)
                {
                    if (this.Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            var emptyPositions = new List<Position>(EmptyPositions());

            if (emptyPositions.Count == 0)
            {
                return;
            }

            var position = emptyPositions[this.random.Next(emptyPositions.Count)];
            this.Grid[position.Row, position.Column] = GridValue.Food;
        }

        private void AddHead(Position position)
        {
            this.snakePositions.AddFirst(position);
            this.Grid[position.Row, position.Column] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            var tail = this.snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            this.snakePositions.RemoveLast();
        }

        private bool IsOutside(Position position)
        {
            return position.Row < 0 || position.Row >= this.Rows || position.Column < 0 || position.Column >= this.Columns;
        }

        private GridValue WillHit(Position newHeadPosition)
        {
            if (IsOutside(newHeadPosition))
            {
                return GridValue.Outside;
            }

            if (newHeadPosition == TailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPosition.Row, newHeadPosition.Column];
        }
    }
}
