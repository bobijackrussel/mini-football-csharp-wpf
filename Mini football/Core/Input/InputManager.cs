using System.Windows.Input;
using Mini_football.Core.Entities;
using Mini_football.Models;

namespace Mini_football.Core.Input;

public class InputManager
{
    private readonly PlayerState _player1;
    private readonly PlayerState _player2;

    public InputManager(PlayerState player1, PlayerState player2)
    {
        _player1 = player1;
        _player2 = player2;
    }

    public void HandleKeyDown(Key key)
    {
        UpdateState(_player1, key, true);
        UpdateState(_player2, key, true);
    }

    public void HandleKeyUp(Key key)
    {
        UpdateState(_player1, key, false);
        UpdateState(_player2, key, false);
    }

    public void Reset()
    {
        _player1.Input.Reset();
        _player2.Input.Reset();
    }

    private static void UpdateState(PlayerState player, Key key, bool isPressed)
    {
        foreach (var binding in player.Controls.Bindings)
        {
            if (binding.Value != key)
            {
                continue;
            }

            switch (binding.Key)
            {
                case PlayerAction.Up:
                    player.Input.Up = isPressed;
                    break;
                case PlayerAction.Down:
                    player.Input.Down = isPressed;
                    break;
                case PlayerAction.Left:
                    player.Input.Left = isPressed;
                    break;
                case PlayerAction.Right:
                    player.Input.Right = isPressed;
                    break;
                case PlayerAction.Kick:
                    player.Input.Kick = isPressed;
                    break;
            }
        }
    }
}
