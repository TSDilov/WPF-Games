using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tetris.Blocks;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            this.imageControls = SetupGameCanvas(this.gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            var imageControls = new Image[grid.Rows, grid.Columns];
            var cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    var imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    var id = grid[r, c];
                    this.imageControls[r, c].Opacity = 1;
                    this.imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (var position in block.TilePositons())
            {
                this.imageControls[position.Row, position.Column].Opacity = 1;
                this.imageControls[position.Row, position.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            var nextBlock = blockQueue.NextBlock;
            NextImage.Source = this.blockImages[nextBlock.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
            {
                HoldImage.Source = this.blockImages[0];
            }
            else
            {
                HoldImage.Source = this.blockImages[heldBlock.Id];
            }
        }

        private void DrawGostBlock(Block block)
        {
            var dropDistance = this.gameState.BlockDropDistance();

            foreach (var position in block.TilePositons())
            {
                this.imageControls[position.Row + dropDistance, position.Column].Opacity = 0.25;
                this.imageControls[position.Row + dropDistance, position.Column].Source = this.tileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(this.gameState.GameGrid);
            DrawGostBlock(this.gameState.CurrentBlock);
            DrawBlock(this.gameState.CurrentBlock);
            DrawNextBlock(this.gameState.BlockQueue);
            DrawHeldBlock(this.gameState.HeldBlock);
            ScoreText.Text = $"Score: {this.gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(this.gameState);

            while (!this.gameState.GameOver)
            {
                var delay = Math.Max(minDelay, maxDelay - (this.gameState.Score * delayDecrease));
                await Task.Delay(delay);
                this.gameState.MoveBlockDown();
                Draw(this.gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {this.gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    this.gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    this.gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    this.gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    this.gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    this.gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    this.gameState.HoldBlock();
                    break;
                case Key.Space:
                    this.gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(this.gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            this.gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}