namespace Mini_football.Core.Input;

public class PlayerInputState
{
    public bool Up { get; set; }
    public bool Down { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Kick { get; set; }

    public bool HasMovementInput => Up || Down || Left || Right;

    public void Reset()
    {
        Up = Down = Left = Right = Kick = false;
    }
}
