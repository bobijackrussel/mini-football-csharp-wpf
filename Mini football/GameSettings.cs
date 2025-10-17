// GameSettings.cs
using Newtonsoft.Json;
using System.Windows.Input;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Mini_football
{
    public class GameSettings
    {
        public Dictionary<string, Key> Player1Controls { get; set; }
        public Dictionary<string, Key> Player2Controls { get; set; }
        
        public GameSettings()
        {
            // Default values for controls
            Player1Controls = new Dictionary<string, Key>
            {
                { "Up", Key.W },
                { "Down", Key.S },
                { "Left", Key.A },
                { "Right", Key.D },
                { "Kick", Key.X }
            };

            Player2Controls = new Dictionary<string, Key>
            {
                { "Up", Key.Up },
                { "Down", Key.Down },
                { "Left", Key.Left },
                { "Right", Key.Right },
                { "Kick", Key.Enter }
            };
        }
        private static string settingsFilePath = "gameSettings.json";

        public string BackgroundImagePath { get; set; }

        public void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }

        // Metoda za učitavanje postavki iz JSON fajla
        public static GameSettings LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var json = File.ReadAllText(settingsFilePath);
                return JsonConvert.DeserializeObject<GameSettings>(json);
            }
          
            return new GameSettings();
        }
    }
}
