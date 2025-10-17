using System;
using Mini_football.Models;

namespace Mini_football.Core.Services;

public class ScoreChangedEventArgs : EventArgs
{
    public ScoreChangedEventArgs(int player1Score, int player2Score)
    {
        Player1Score = player1Score;
        Player2Score = player2Score;
    }

    public int Player1Score { get; }
    public int Player2Score { get; }
}

public class GameEndedEventArgs : EventArgs
{
    public GameEndedEventArgs(string winnerId, bool isDraw)
    {
        WinnerId = winnerId;
        IsDraw = isDraw;
    }

    public string WinnerId { get; }
    public bool IsDraw { get; }
}

public class ScoreTracker
{
    private readonly GameSettings _settings;

    public ScoreTracker(GameSettings settings)
    {
        _settings = settings;
    }

    public int Player1Score { get; private set; }

    public int Player2Score { get; private set; }

    public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;

    public event EventHandler<GameEndedEventArgs>? GameEnded;

    public void RegisterGoal(GoalOwner owner)
    {
        if (owner == GoalOwner.Player1)
        {
            Player1Score++;
        }
        else
        {
            Player2Score++;
        }

        ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(Player1Score, Player2Score));

        if (_settings.GameMode == GameMode.GoldenGoal)
        {
            var winner = owner == GoalOwner.Player1 ? "Player1" : "Player2";
            GameEnded?.Invoke(this, new GameEndedEventArgs(winner, false));
        }
    }

    public GameEndedEventArgs EvaluateWinner()
    {
        if (Player1Score == Player2Score)
        {
            return new GameEndedEventArgs("Draw", true);
        }

        return new GameEndedEventArgs(Player1Score > Player2Score ? "Player1" : "Player2", false);
    }

    public void Reset()
    {
        Player1Score = 0;
        Player2Score = 0;
        ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(Player1Score, Player2Score));
    }
}
