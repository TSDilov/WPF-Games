using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 },
        };

        private readonly int rows = 20, cols = 20;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;

        public MainWindow()
        {
            InitializeComponent();
            this.gridImages = SetUpGrid();
            this.gameState = new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            this.gameState = new GameState(rows, cols);
        }


        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!this.gameRunning)
            {
                this.gameRunning = true;
                await RunGame();
                this.gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.gameState.GameOver)
            {
                return;
            }

            switch (e.Key) 
            {
                case Key.Left:
                    this.gameState.ChangeDirection(Direction.Left); 
                    break;
                case Key.Right:
                    this.gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    this.gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    this.gameState.ChangeDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!this.gameState.GameOver) 
            {
                await Task.Delay(100);
                this.gameState.Move();
                Draw();
            }
        }

        private Image[,] SetUpGrid()
        {
            var images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {this.gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var gridVal = this.gameState.Grid[r, c];
                    this.gridImages[r, c].Source = this.gridValToImage[gridVal];
                    this.gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            var headPosition = this.gameState.HeadPosition();
            var image = this.gridImages[headPosition.Row, headPosition.Column];
            image.Source = Images.Head;

            var rotation = dirToRotation[this.gameState.Direction];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            var positions = new List<Position>(this.gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                var position = positions[i];
                var source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                this.gridImages[position.Row, position.Column].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to start";
        }
    }
}