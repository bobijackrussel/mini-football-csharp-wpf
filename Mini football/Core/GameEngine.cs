using System;
using System.Windows;
using Mini_football.Core.Entities;
using Mini_football.Core.Field;
using Mini_football.Core.Input;
using Mini_football.Core.Services;
using Mini_football.Models;

namespace Mini_football.Core;

public class TimeChangedEventArgs : EventArgs
{
    public TimeChangedEventArgs(TimeSpan remaining)
    {
        Remaining = remaining;
    }

    public TimeSpan Remaining { get; }
}

public class GoalScoredEventArgs : EventArgs
{
    public GoalScoredEventArgs(string scoringPlayer)
    {
        ScoringPlayer = scoringPlayer;
    }

    public string ScoringPlayer { get; }
}

public class GameEngine
{
    private readonly GameField _field;
    private readonly PlayerState _player1;
    private readonly PlayerState _player2;
    private readonly BallState _ball;
    private readonly InputManager _inputManager;
    private readonly GoalDetector _goalDetector;
    private readonly ScoreTracker _scoreTracker;
    private readonly GameSettings _settings;

    private TimeSpan _remainingTime;

    public GameEngine(GameField field, PlayerState player1, PlayerState player2, BallState ball, GoalDetector goalDetector, ScoreTracker scoreTracker, GameSettings settings)
    {
        _field = field;
        _player1 = player1;
        _player2 = player2;
        _ball = ball;
        _goalDetector = goalDetector;
        _scoreTracker = scoreTracker;
        _settings = settings;
        _inputManager = new InputManager(player1, player2);

        _remainingTime = TimeSpan.FromSeconds(settings.ClassicalDurationSeconds);

        _scoreTracker.ScoreChanged += (_, args) => ScoreChanged?.Invoke(this, args);
        _scoreTracker.GameEnded += (_, args) =>
        {
            IsMatchOver = true;
            MatchEnded?.Invoke(this, args);
        };
    }

    public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;
    public event EventHandler<GoalScoredEventArgs>? GoalScored;
    public event EventHandler<GameEndedEventArgs>? MatchEnded;
    public event EventHandler<TimeChangedEventArgs>? TimeChanged;

    public bool IsPaused { get; private set; }

    public bool IsMatchOver { get; private set; }

    public void HandleKeyDown(System.Windows.Input.Key key)
    {
        if (IsPaused || IsMatchOver)
        {
            return;
        }

        _inputManager.HandleKeyDown(key);
    }

    public void HandleKeyUp(System.Windows.Input.Key key)
    {
        _inputManager.HandleKeyUp(key);
    }

    public void Update(TimeSpan delta)
    {
        if (IsPaused || IsMatchOver)
        {
            return;
        }

        MovementService.UpdatePlayerPosition(_player1, _field.Bounds);
        MovementService.UpdatePlayerPosition(_player2, _field.Bounds);

        CollisionService.ResolvePlayerCollisions(_player1, _player2);
        CollisionService.ResolveObstacleCollisions(_player1, _field.Obstacles);
        CollisionService.ResolveObstacleCollisions(_player2, _field.Obstacles);

        if (_player1.Input.Kick && CollisionService.IsBallNearPlayer(_ball, _player1))
        {
            KickBallFromPlayer(_player1);
        }
        else if (_player2.Input.Kick && CollisionService.IsBallNearPlayer(_ball, _player2))
        {
            KickBallFromPlayer(_player2);
        }

        CollisionService.ResolveBallPlayerCollision(_ball, _player1);
        CollisionService.ResolveBallPlayerCollision(_ball, _player2);
        CollisionService.ResolveObstacleCollisions(_ball, _field.Obstacles);

        MovementService.UpdateBallPosition(_ball, _field.Bounds);
        _ball.ApplyFriction(0.95);

        var goal = _goalDetector.DetectGoal(_ball);
        if (goal != null)
        {
            OnGoalScored(goal.Value);
        }

        if (_settings.GameMode == GameMode.Classical)
        {
            UpdateTimer(delta);
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Resume()
    {
        IsPaused = false;
    }

    public void ResetAfterGoal()
    {
        _ball.Reset();
        _player1.ResetPosition();
        _player2.ResetPosition();
        _inputManager.Reset();
    }

    public void Restart()
    {
        IsMatchOver = false;
        _scoreTracker.Reset();
        ResetAfterGoal();
        _remainingTime = TimeSpan.FromSeconds(_settings.ClassicalDurationSeconds);
        TimeChanged?.Invoke(this, new TimeChangedEventArgs(_remainingTime));
    }

    private void KickBallFromPlayer(PlayerState player)
    {
        var direction = _ball.Center - player.Center;
        if (direction.Length <= 0)
        {
            return;
        }

        direction.Normalize();
        _ball.Velocity = direction * 10;
        _ball.Velocity += new Vector(0, direction.Y * 2);
    }

    private void OnGoalScored(GoalOwner owner)
    {
        var scoringPlayer = owner == GoalOwner.Player1 ? "Player1" : "Player2";
        GoalScored?.Invoke(this, new GoalScoredEventArgs(scoringPlayer));
        _scoreTracker.RegisterGoal(owner);

        if (!IsMatchOver)
        {
            ResetAfterGoal();
        }
    }

    private void UpdateTimer(TimeSpan delta)
    {
        if (_remainingTime <= TimeSpan.Zero)
        {
            return;
        }

        _remainingTime -= delta;
        if (_remainingTime < TimeSpan.Zero)
        {
            _remainingTime = TimeSpan.Zero;
        }

        TimeChanged?.Invoke(this, new TimeChangedEventArgs(_remainingTime));

        if (_remainingTime == TimeSpan.Zero && !IsMatchOver)
        {
            IsMatchOver = true;
            var result = _scoreTracker.EvaluateWinner();
            MatchEnded?.Invoke(this, result);
        }
    }

    public void EndMatch(GameEndedEventArgs result)
    {
        IsMatchOver = true;
        MatchEnded?.Invoke(this, result);
    }
}
