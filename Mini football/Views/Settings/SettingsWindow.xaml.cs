using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Mini_football.Models;

namespace Mini_football.Views.Settings
{
    public partial class SettingsWindow : Window
    {
        private readonly HashSet<Key> assignedKeys = new();
        private Button? activeButton;
        private readonly GameSettings gameSettings;

        public SettingsWindow(GameSettings settings)
        {
            gameSettings = settings;
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            assignedKeys.Clear();

            foreach (var binding in gameSettings.Player1Controls.Bindings)
            {
                if (TryFindButton("Player1", binding.Key, out var button))
                {
                    button.Content = GetKeySymbol(binding.Value);
                }
                assignedKeys.Add(binding.Value);
            }

            foreach (var binding in gameSettings.Player2Controls.Bindings)
            {
                if (TryFindButton("Player2", binding.Key, out var button))
                {
                    button.Content = GetKeySymbol(binding.Value);
                }
                assignedKeys.Add(binding.Value);
            }
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
            }

            activeButton = sender as Button;
            if (activeButton == null)
            {
                return;
            }

            activeButton.Background = Brushes.Gray;
            PreviewKeyDown += SettingsWindow_PreviewKeyDown;
            MouseDown += SettingsWindow_MouseDown;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (activeButton == null)
            {
                return;
            }

            var pressedKey = e.Key;

            if (!TryParseButton(activeButton.Name, out var playerIndex, out var action))
            {
                return;
            }

            var currentKey = GetCurrentKey(playerIndex, action);
            if (assignedKeys.Contains(pressedKey) && pressedKey != currentKey)
            {
                MessageBox.Show("Ovaj taster je već zauzet. Izaberite drugi.");
                return;
            }

            assignedKeys.Remove(currentKey);
            assignedKeys.Add(pressedKey);

            activeButton.Content = GetKeySymbol(pressedKey);
            UpdateGameSettings(playerIndex, action, pressedKey);

            activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
            activeButton = null;
            PreviewKeyDown -= SettingsWindow_PreviewKeyDown;
            MouseDown -= SettingsWindow_MouseDown;
        }

        private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
                activeButton = null;
            }

            PreviewKeyDown -= SettingsWindow_PreviewKeyDown;
            MouseDown -= SettingsWindow_MouseDown;
        }

        private bool TryFindButton(string playerPrefix, PlayerAction action, out Button? button)
        {
            button = FindName($"{playerPrefix}_{action}Button") as Button;
            return button != null;
        }

        private bool TryParseButton(string buttonName, out int playerIndex, out PlayerAction action)
        {
            playerIndex = 0;
            action = PlayerAction.Up;

            if (string.IsNullOrWhiteSpace(buttonName))
            {
                return false;
            }

            var parts = buttonName.Split('_');
            if (parts.Length < 2)
            {
                return false;
            }

            if (!parts[0].StartsWith("Player", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!int.TryParse(parts[0].Substring("Player".Length), out playerIndex))
            {
                return false;
            }

            var actionName = parts[1].Replace("Button", string.Empty);
            return Enum.TryParse(actionName, out action);
        }

        private Key GetCurrentKey(int playerIndex, PlayerAction action)
        {
            return playerIndex switch
            {
                1 => gameSettings.Player1Controls[action],
                2 => gameSettings.Player2Controls[action],
                _ => Key.None
            };
        }

        private void UpdateGameSettings(int playerIndex, PlayerAction action, Key key)
        {
            switch (playerIndex)
            {
                case 1:
                    gameSettings.Player1Controls[action] = key;
                    break;
                case 2:
                    gameSettings.Player2Controls[action] = key;
                    break;
            }
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
    }
}
