using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        public StartMenu()
        {
            InitializeComponent();
            gameSettings = new GameSettings();
            gameSettings.BackgroundImagePath = _images[0];
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
            Console.WriteLine(gameSettings.BackgroundImagePath);
            //gameSettings.SaveSettings();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmQuitWindow confirmWindow = new ConfirmQuitWindow();

            confirmWindow.Owner = this;
            confirmWindow.ShowDialog();

            
            if (confirmWindow.UserConfirmedExit)
            {
                Application.Current.Shutdown();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //gameSettings.SaveSettings();
            SettingsWindow setingsWindow = new SettingsWindow(gameSettings);
            
            setingsWindow.Owner = this;
            setingsWindow.ShowDialog();
       
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            //gameSettings.SaveSettings();
            MainWindow mainWindow = new MainWindow(gameSettings);
            
            mainWindow.Owner = this;
            mainWindow.ShowDialog();
        }
    }
}
