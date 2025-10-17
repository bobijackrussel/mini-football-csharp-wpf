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
using System.Windows.Threading;

namespace Mini_football
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window
    {
        DispatcherTimer dispatcherTimer=new DispatcherTimer();

        public LoadingScreen()
        {
            InitializeComponent();
            dispatcherTimer.Tick +=new  EventHandler(Loading);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();
        }
        private void Loading(Object? sender, EventArgs e)
        {
            StartMenu main= new StartMenu();
            main.Show();

            dispatcherTimer.Stop();
            this.Close();
        }
    }
}
