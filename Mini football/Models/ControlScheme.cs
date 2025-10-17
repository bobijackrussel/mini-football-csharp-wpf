using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Mini_football.Models;

public class ControlScheme
{
    private readonly Dictionary<PlayerAction, Key> _bindings = new();

    public ControlScheme()
    {
    }

    public ControlScheme(IEnumerable<KeyValuePair<PlayerAction, Key>> bindings)
    {
        foreach (var binding in bindings)
        {
            _bindings[binding.Key] = binding.Value;
        }
    }

    public Key this[PlayerAction action]
    {
        get => _bindings[action];
        set => _bindings[action] = value;
    }

    public IReadOnlyDictionary<PlayerAction, Key> Bindings => _bindings;

    public static ControlScheme CreateDefaultPlayerOne() => new(new[]
    {
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Up, Key.W),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Down, Key.S),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Left, Key.A),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Right, Key.D),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Kick, Key.X)
    });

    public static ControlScheme CreateDefaultPlayerTwo() => new(new[]
    {
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Up, Key.Up),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Down, Key.Down),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Left, Key.Left),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Right, Key.Right),
        new KeyValuePair<PlayerAction, Key>(PlayerAction.Kick, Key.Enter)
    });

    public Dictionary<string, Key> ToLegacyDictionary()
    {
        return _bindings.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value);
    }

    public static ControlScheme FromLegacyDictionary(IDictionary<string, Key> bindings)
    {
        var mappedBindings = bindings
            .Where(kvp => Enum.TryParse<PlayerAction>(kvp.Key, out _))
            .Select(kvp => new KeyValuePair<PlayerAction, Key>((PlayerAction)Enum.Parse(typeof(PlayerAction), kvp.Key), kvp.Value));

        var scheme = new ControlScheme(mappedBindings);

        // Ensure all actions have a key even if source dictionary was incomplete
        foreach (PlayerAction action in Enum.GetValues(typeof(PlayerAction)))
        {
            if (!scheme._bindings.ContainsKey(action))
            {
                scheme._bindings[action] = action switch
                {
                    PlayerAction.Up => Key.W,
                    PlayerAction.Down => Key.S,
                    PlayerAction.Left => Key.A,
                    PlayerAction.Right => Key.D,
                    PlayerAction.Kick => Key.X,
                    _ => Key.None
                };
            }
        }

        return scheme;
    }
}
