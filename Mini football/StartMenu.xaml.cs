using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Mini_football.Models;

namespace Mini_football
{
    public partial class StartMenu : Window
    {
        private int _currentImageIndex = 0;
        private GameSettings gameSettings;

        private readonly string[] _images =
        {
            @".\img\pitch\pitch1.png",
            @".\img\pitch\pitch2.png",
            @".\img\pitch\pitch3.png",
            @".\img\pitch\pitch4.png",
            @".\img\pitch\pitch5.png",
            @".\img\pitch\pitch6.png",
            @".\img\pitch\pitch7.png",
            @".\img\pitch\pitch8.png",
            @".\img\pitch\pitch0.png"
        };

        private readonly List<BallOption> _ballOptions = new()
        {
            new BallOption("Classic", @".\img\ball\ball0.png"),
            new BallOption("Retro", @".\img\ball\ball1.png"),
            new BallOption("Standard", @".\img\ball\ball2.png"),
            new BallOption("Yellow", @".\img\ball\ball3.png"),
            new BallOption("Red", @".\img\ball\ball4.png"),
            new BallOption("Blue", @".\img\ball\ball5.png"),
            new BallOption("Green", @".\img\ball\ball6.png"),
            new BallOption("Shadow", @".\img\ball\ball7.png"),
            new BallOption("Galaxy", @".\img\ball\ball8.png"),
            new BallOption("Neon", @".\img\ball\ball9.png"),
            new BallOption("Night", @".\img\ball\ball10.png"),
            new BallOption("Sunrise", @".\img\ball\ball11.png")
        };

        public StartMenu()
        {
            InitializeComponent();
            gameSettings = GameSettings.Load();

            InitializeSelections();
        }

        private void InitializeSelections()
        {
            if (!string.IsNullOrWhiteSpace(gameSettings.BackgroundImagePath))
            {
                var index = Array.FindIndex(_images, img => img.Equals(gameSettings.BackgroundImagePath, StringComparison.OrdinalIgnoreCase));
                _currentImageIndex = index >= 0 ? index : 0;
            }

            backgroundImage.Source = new BitmapImage(new Uri(_images[_currentImageIndex], UriKind.Relative));

            modeComboBox.ItemsSource = Enum.GetValues(typeof(GameMode));
            modeComboBox.SelectedItem = gameSettings.GameMode;

            ballComboBox.ItemsSource = _ballOptions;
            var selectedBall = _ballOptions.FirstOrDefault(option => option.Path.Equals(gameSettings.BallImagePath, StringComparison.OrdinalIgnoreCase))
                               ?? _ballOptions.First();
            ballComboBox.SelectedItem = selectedBall;
            ballPreview.ImageSource = new BitmapImage(new Uri(selectedBall.Path, UriKind.Relative));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var clickPosition = e.GetPosition(this);

            if (clickPosition.X < this.ActualWidth / 2)
            {
                _currentImageIndex = (_currentImageIndex - 1 + _images.Length) % _images.Length;
            }
            else
            {
                _currentImageIndex = (_currentImageIndex + 1) % _images.Length;
            }
            backgroundImage.Source = new BitmapImage(new Uri(_images[_currentImageIndex], UriKind.Relative));
            gameSettings.BackgroundImagePath = _images[_currentImageIndex];
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmQuitWindow confirmWindow = new ConfirmQuitWindow();

            confirmWindow.Owner = this;
            confirmWindow.ShowDialog();

            if (confirmWindow.UserConfirmedExit)
            {
                gameSettings.Save();
                Application.Current.Shutdown();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setingsWindow = new SettingsWindow(gameSettings);

            setingsWindow.Owner = this;
            setingsWindow.ShowDialog();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            gameSettings.Save();
            MainWindow mainWindow = new MainWindow(gameSettings);

            mainWindow.Owner = this;
            mainWindow.ShowDialog();
        }

        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modeComboBox.SelectedItem is GameMode mode)
            {
                gameSettings.GameMode = mode;
            }
        }

        private void BallComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ballComboBox.SelectedItem is BallOption option)
            {
                gameSettings.BallImagePath = option.Path;
                ballPreview.ImageSource = new BitmapImage(new Uri(option.Path, UriKind.Relative));
            }
        }

        private record BallOption(string Name, string Path);
    }
}
