using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mini_football
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
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
            this.Close();
            
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
