using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tic_Tac_Toe.Enums;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imageSources = new()
        {
            { Player.X, new BitmapImage(new Uri("pack://application:,,,/Assets/X15.png")) },
            { Player.O, new BitmapImage(new Uri("pack://application:,,,/Assets/O15.png")) }
        };

        private readonly Dictionary<Player, ObjectAnimationUsingKeyFrames> animations = new()
        {
            { Player.X, new ObjectAnimationUsingKeyFrames() },
            { Player.O, new ObjectAnimationUsingKeyFrames() }
        };

        private readonly DoubleAnimation fadeOutAnimation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(.5),
            From = 1,
            To = 0
        };

        private readonly DoubleAnimation fadeInAnimation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(.5),
            From = 0,
            To = 1
        };

        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly GameState gameState = new GameState();


        public MainWindow()
        {
            InitializeComponent();
            SetupGameGrid();
            SetupAnimations();

            this.gameState.MoveMade += OnMoveMade;
            this.gameState.GameEnded += OnGameEnded;
            this.gameState.GameRestarted += OnGameRestarted;
        }

        private void SetupGameGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    this.imageControls[r, c] = imageControl;
                }
            }
        }

        private void SetupAnimations()
        {
            this.animations[Player.X].Duration = TimeSpan.FromSeconds(.25);
            this.animations[Player.O].Duration = TimeSpan.FromSeconds(.25);

            for (int i = 0; i < 16; i++)
            {
                var xUri = new Uri($"pack://application:,,,/Assets/X{i}.png");
                var xImg = new BitmapImage(xUri);
                var xKeyFrame = new DiscreteObjectKeyFrame(xImg);
                this.animations[Player.X].KeyFrames.Add(xKeyFrame);
                    
                var oUri = new Uri($"pack://application:,,,/Assets/O{i}.png");
                var oImg = new BitmapImage(oUri);
                var oKeyFrame = new DiscreteObjectKeyFrame(oImg);
                this.animations[Player.O].KeyFrames.Add(oKeyFrame);
            }
        }

        private async Task FadeOut(UIElement uiElement)
        {
            uiElement.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            uiElement.Visibility = Visibility.Hidden;
        }

        private async Task FadeIn(UIElement uiElement)
        {
            uiElement.Visibility = Visibility.Visible;
            uiElement.BeginAnimation(OpacityProperty, fadeInAnimation);
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);
        }

        private async Task TransitionToEndScreen(string text, ImageSource winnerImage)
        {
            await Task.WhenAll(FadeOut(TurnPanel), FadeOut(GameCanvas));
            ResultText.Text = text;
            WinnerImage.Source = winnerImage;
            await FadeIn(EndScreen);
        }

        private async Task TransitionToGameScreen()
        {
            await FadeOut(EndScreen);
            Line.Visibility= Visibility.Hidden;
            await Task.WhenAll(FadeIn(TurnPanel), FadeIn(GameCanvas));
        }

        private (Point, Point) FindLinePoints(WinInfo winInfo)
        {
            var squareSize = GameGrid.Width / 3;
            var margin = squareSize / 2;

            if (winInfo.Type == WinType.Row)
            {
                var y = winInfo.Number * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y));
            }
            if (winInfo.Type == WinType.Column)
            {
                var x = winInfo.Number * squareSize + margin;
                return (new Point(x, 0), new Point(x, GameGrid.Height));
            }
            if (winInfo.Type == WinType.MainDiagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }

            return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
        }

        private async Task ShowLine(WinInfo winInfo)
        {
            (var start, var end) = FindLinePoints(winInfo);
            Line.X1 = start.X;
            Line.Y1 = start.Y;

            var x2Animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.25),
                From = start.X,
                To = end.X
            };

            var y2Animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.25),
                From = start.Y,
                To = end.Y
            };

            Line.Visibility = Visibility.Visible;
            Line.BeginAnimation(Line.X2Property, x2Animation);
            Line.BeginAnimation(Line.Y2Property, y2Animation);
            await Task.Delay(x2Animation.Duration.TimeSpan);
        }

        private void OnMoveMade(int r, int c)
        {
            var player = this.gameState.GameGrid[r, c];
            this.imageControls[r, c].BeginAnimation(Image.SourceProperty, this.animations[player]);
            PlayerImage.Source = this.imageSources[this.gameState.CurrentPlayer];
        }

        private async void OnGameEnded(GameResult gameResult)
        {
            await Task.Delay(1000);
            if (gameResult.Winner == Player.None)
            {
                await TransitionToEndScreen("It's a tie!", null);
            }
            else
            {
                await ShowLine(gameResult.WinInfo);
                await Task.Delay(1000);
                await TransitionToEndScreen("Winner: ", this.imageSources[gameResult.Winner]);
            }
        }

        private async void OnGameRestarted()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    this.imageControls[r, c].BeginAnimation(Image.SourceProperty, null);
                    this.imageControls[r, c].Source = null;
                }
            }

            PlayerImage.Source = this.imageSources[this.gameState.CurrentPlayer];
            await TransitionToGameScreen();
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var squareSize = GameGrid.Width / 3;
            var clickPosition = e.GetPosition(GameGrid);
            var row = (int)(clickPosition.Y / squareSize);
            var column = (int)(clickPosition.X / squareSize);
            this.gameState.MakeMove(row, column);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.gameState.GameOver)
            {
                this.gameState.Reset();
            }
        }
    }
}