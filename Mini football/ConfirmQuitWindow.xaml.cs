using System.Windows;

namespace Mini_football
{
    public partial class ConfirmQuitWindow : Window
    {
        public bool UserConfirmedExit { get; private set; }

        public ConfirmQuitWindow()
        {
            InitializeComponent();
            UserConfirmedExit = false;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmedExit = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
