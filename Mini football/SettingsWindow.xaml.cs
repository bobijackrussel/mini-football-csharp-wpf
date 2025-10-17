using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mini_football
{
    public partial class SettingsWindow : Window
    {
        private Dictionary<string, Key> assignedKeys = new Dictionary<string, Key>();
        private Button activeButton = null;
        private GameSettings gameSettings;

        public SettingsWindow(GameSettings settings)
        {
            gameSettings = settings;
            InitializeComponent();
            InitializeControls();
        }

         private void InitializeControls()
        {
            foreach (var control in gameSettings.Player1Controls)
            {
                Button button = this.FindName($"Player1_{control.Key}Button") as Button;
                if (button != null) button.Content = control.Value.ToString();
            }

            foreach (var control in gameSettings.Player2Controls)
            {
                Button button = this.FindName($"Player2_{control.Key}Button") as Button;
                if (button != null) button.Content = control.Value.ToString();
            }
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
            }

            activeButton = sender as Button;
            activeButton.Background = Brushes.Gray; 
            this.PreviewKeyDown += SettingsWindow_PreviewKeyDown;
            this.MouseDown += SettingsWindow_MouseDown;
        }

        // Povratak na StartMenu
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //gameSettings.SaveSettings();
            var startMenu = new StartMenu();
            this.Close();
        }

        // Obrada unosa sa tastature za aktivno dugme
        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (activeButton == null) return;

            Key pressedKey = e.Key;

            if (assignedKeys.ContainsValue(pressedKey))
            {
                MessageBox.Show("Ovaj taster je već zauzet. Izaberite drugi.");
                return;
            }

            // Ažuriranje teksta na dugmetu
            activeButton.Content = pressedKey.ToString();
            assignedKeys[activeButton.Name] = pressedKey;

            // Ažuriranje gameSettings u zavisnosti od dugmeta koje se ažurira
            UpdateGameSettings(activeButton.Name, pressedKey);
            //gameSettings.SaveSettings();
            
            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
            activeButton = null;
            this.PreviewKeyDown -= SettingsWindow_PreviewKeyDown;
            this.MouseDown -= SettingsWindow_MouseDown;
        }

        // Ažuriraj gameSettings prema dugmetu koje je aktivno
        private void UpdateGameSettings(string buttonName, Key key)
        {
            if (buttonName.StartsWith("Player1_"))
            {
                string control = buttonName.Replace("Player1_", "").Replace("Button", "");
                gameSettings.Player1Controls[control] = key;
            }
            else if (buttonName.StartsWith("Player2_"))
            {
                string control = buttonName.Replace("Player2_", "").Replace("Button", "");
                gameSettings.Player2Controls[control] = key;
            }
        }
       

        // Obrada klika mišem izvan aktivnog dugmeta za otkazivanje unosa
        private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                activeButton = null;
            }

            this.PreviewKeyDown -= SettingsWindow_PreviewKeyDown;
            this.MouseDown -= SettingsWindow_MouseDown;
        }
    }
}
