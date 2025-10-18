using System;
using System.Windows;
using System.Windows.Threading;
using Mini_football.Views.Menus;

namespace Mini_football.Views.Loading
{
    public partial class LoadingScreen : Window
    {
        private readonly DispatcherTimer _dispatcherTimer = new();

        public LoadingScreen()
        {
            InitializeComponent();
            _dispatcherTimer.Tick += OnLoadingComplete;
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(10);
            _dispatcherTimer.Start();
        }

        private void OnLoadingComplete(object? sender, EventArgs e)
        {
            var startMenu = new StartMenu();
            startMenu.Show();

            _dispatcherTimer.Stop();
            Close();
        }
    }
}
