using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Mini_football.Core;
using Mini_football.Core.Entities;
using Mini_football.Core.Field;
using Mini_football.Core.Services;
using Mini_football.Models;
using Mini_football.Views.Dialogs;

namespace Mini_football.Views.Game
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _gameLoopTimer;
        private readonly GameSettings _gameSettings;
        private GameEngine? _gameEngine;

        public MainWindow(GameSettings settings)
        {
            InitializeComponent();

            _gameSettings = settings;
            _gameLoopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _gameLoopTimer.Tick += OnGameLoopTick;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyVisualSettings();
            InitializeGameEngine();
            footballField.Focus();

            _gameEngine?.Restart();
            _gameEngine?.Resume();
            _gameLoopTimer.Start();
        }

        private void ApplyVisualSettings()
        {
            var background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(_gameSettings.BackgroundImagePath, UriKind.Relative)),
                Stretch = Stretch.Fill
            };
            footballField.Background = background;

            ball.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(_gameSettings.BallImagePath, UriKind.Relative)),
                Stretch = Stretch.UniformToFill
            };

            SetPlayerControls();
            UpdateScoreboard(0, 0);
        }

        private void InitializeGameEngine()
        {
            UpdateLayout();

            var obstacles = new List<FrameworkElement>
            {
                Goal1_frontPost_Top,
                Goal1_frontPost_Bottom,
                Goal1_backNet,
                Goal1_SidePostTop,
                Goal1_SidePostBottom,
                Goal2_frontPost_Top,
                Goal2_frontPost_Bottom,
                Goal2_backNet,
                Goal2_sidePostTop,
                Goal2_sidePostBottom
            };

            var field = new GameField(footballField, obstacles);
            var player1State = new PlayerState(player1, _gameSettings.Player1Controls);
            var player2State = new PlayerState(player2, _gameSettings.Player2Controls);
            var ballState = new BallState(ball);

            var player1Goal = new GoalDefinition(
                Goal1_line,
                Goal1_SidePostTop,
                Goal1_SidePostBottom,
                new[] { Goal1_frontPost_Top, Goal1_frontPost_Bottom, Goal1_backNet },
                isLeftGoal: true);

            var player2Goal = new GoalDefinition(
                Goal2_line,
                Goal2_sidePostTop,
                Goal2_sidePostBottom,
                new[] { Goal2_frontPost_Top, Goal2_frontPost_Bottom, Goal2_backNet },
                isLeftGoal: false);

            var goalDetector = new GoalDetector(player1Goal, player2Goal, ball.RenderSize.Width);
            var scoreTracker = new ScoreTracker(_gameSettings);

            _gameEngine = new GameEngine(field, player1State, player2State, ballState, goalDetector, scoreTracker, _gameSettings);
            _gameEngine.ScoreChanged += (_, args) => UpdateScoreboard(args.Player1Score, args.Player2Score);
            _gameEngine.GoalScored += async (_, args) => await ShowGoalMessage(args.ScoringPlayer);
            _gameEngine.MatchEnded += OnMatchEnded;
            _gameEngine.TimeChanged += (_, args) => UpdateTimer(args.Remaining);
        }

        private void OnGameLoopTick(object? sender, EventArgs e)
        {
            _gameEngine?.Update(_gameLoopTimer.Interval);
        }

        private async Task ShowGoalMessage(string scoringPlayer)
        {
            goal.Visibility = Visibility.Visible;
            Message.Visibility = Visibility.Visible;
            var displayName = GetDisplayName(scoringPlayer);
            Message.Content = $"Gol! {displayName}";

            await Task.Delay(1000);

            goal.Visibility = Visibility.Collapsed;
            Message.Visibility = Visibility.Collapsed;
            Message.Content = "GoooL!";
        }

        private void UpdateScoreboard(int player1Score, int player2Score)
        {
            scoreboard.Content = $"{player1Score} - {player2Score}";
        }

        private void UpdateTimer(TimeSpan remaining)
        {
            if (_gameSettings.GameMode == GameMode.GoldenGoal)
            {
                timerLabel.Content = "∞";
                return;
            }

            timerLabel.Content = remaining > TimeSpan.Zero
                ? remaining.ToString(@"m\:ss")
                : "00:00";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            _gameEngine?.HandleKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            _gameEngine?.HandleKeyUp(e.Key);
        }

        private void OnMatchEnded(object? sender, GameEndedEventArgs e)
        {
            _gameLoopTimer.Stop();
            _gameEngine?.Pause();

            var resultText = e.IsDraw
                ? "Neriješeno!"
                : $"{GetDisplayName(e.WinnerId)} je pobijedio!";

            var winWindow = new WinWindow(resultText)
            {
                Owner = this
            };

            winWindow.ShowDialog();

            if (winWindow.UserConfirmedExit)
            {
                Close();
                return;
            }

            if (winWindow.UserConfirmedRestart)
            {
                _gameEngine?.Restart();
                _gameEngine?.Resume();
                _gameLoopTimer.Start();
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            _gameEngine?.Pause();
            _gameLoopTimer.Stop();

            var pauseWindow = new PauseWindow
            {
                Owner = this
            };

            pauseWindow.ShowDialog();

            if (pauseWindow.UserConfirmedExit)
            {
                Close();
                return;
            }

            _gameEngine?.Resume();
            _gameLoopTimer.Start();
        }

        private void SetPlayerControls()
        {
            Player1UpButton.Content = GetKeySymbol(_gameSettings.Player1Controls[PlayerAction.Up]);
            Player1DownButton.Content = GetKeySymbol(_gameSettings.Player1Controls[PlayerAction.Down]);
            Player1LeftButton.Content = GetKeySymbol(_gameSettings.Player1Controls[PlayerAction.Left]);
            Player1RightButton.Content = GetKeySymbol(_gameSettings.Player1Controls[PlayerAction.Right]);
            Player1KickButton.Content = GetKeySymbol(_gameSettings.Player1Controls[PlayerAction.Kick]);

            Player2UpButton.Content = GetKeySymbol(_gameSettings.Player2Controls[PlayerAction.Up]);
            Player2DownButton.Content = GetKeySymbol(_gameSettings.Player2Controls[PlayerAction.Down]);
            Player2LeftButton.Content = GetKeySymbol(_gameSettings.Player2Controls[PlayerAction.Left]);
            Player2RightButton.Content = GetKeySymbol(_gameSettings.Player2Controls[PlayerAction.Right]);
            Player2KickButton.Content = GetKeySymbol(_gameSettings.Player2Controls[PlayerAction.Kick]);
        }

        private static string GetKeySymbol(Key key)
        {
            return key switch
            {
                Key.Left => "←",
                Key.Right => "→",
                Key.Up => "↑",
                Key.Down => "↓",
                Key.Enter => "⏎",
                Key.Space => "␣",
                _ => key.ToString()
            };
        }

        private static string GetDisplayName(string playerId)
        {
            return playerId switch
            {
                "Player1" => "Player 1",
                "Player2" => "Player 2",
                _ => playerId
            };
        }
    }
}
