using System.Windows;

namespace Mini_football.Views.Dialogs
{
    public partial class PauseWindow : Window
    {
        public bool UserConfirmedExit { get; private set; }

        public PauseWindow()
        {
            InitializeComponent();
            UserConfirmedExit = false;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedExit = true;
            Close();

        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
