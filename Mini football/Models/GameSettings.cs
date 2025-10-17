using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Input;

namespace Mini_football.Models;

public class GameSettings
{
    private const string SettingsFilePath = "gameSettings.json";

    [JsonConverter(typeof(StringEnumConverter))]
    public GameMode GameMode { get; set; } = GameMode.Classical;

    public ControlScheme Player1Controls { get; set; } = ControlScheme.CreateDefaultPlayerOne();

    public ControlScheme Player2Controls { get; set; } = ControlScheme.CreateDefaultPlayerTwo();

    public string BackgroundImagePath { get; set; } = @".\img\pitch\pitch1.png";

    public string BallImagePath { get; set; } = @".\img\ball\ball2.png";

    public int ClassicalDurationSeconds { get; set; } = 60;

    public static GameSettings Load()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new GameSettings();
        }

        var json = File.ReadAllText(SettingsFilePath);
        var deserialized = JsonConvert.DeserializeObject<GameSettings>(json);
        return deserialized ?? new GameSettings();
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(SettingsFilePath, json);
    }

    public void ApplyLegacyDictionaries(IDictionary<string, Key> playerOne, IDictionary<string, Key> playerTwo)
    {
        Player1Controls = ControlScheme.FromLegacyDictionary(playerOne);
        Player2Controls = ControlScheme.FromLegacyDictionary(playerTwo);
    }
}
