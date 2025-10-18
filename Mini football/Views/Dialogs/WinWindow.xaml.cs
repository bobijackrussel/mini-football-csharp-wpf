using System;
using System.Windows;

namespace Mini_football.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for WinWindow.xaml
    /// </summary>
    public partial class WinWindow : Window
    {
        // Ova promenljiva će služiti da saznamo da li je korisnik želeo da se vrati na meni
        public bool UserConfirmedExit   { get; private set; }
        public bool UserConfirmedRestart { get; private set; }

        public WinWindow(string message)
        {
            InitializeComponent();
            UserConfirmedExit = false;

            // Postavimo tekst koji prikazuje rezultat partije
            winnerMessage.Text = message;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Postavljamo indikator da korisnik želi povratak na početni meni
            UserConfirmedExit = true;
            Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // Resetuj igru vraćanjem svih parametara u početno stanje
            UserConfirmedRestart = true;
            Close();
        }
    }
}

