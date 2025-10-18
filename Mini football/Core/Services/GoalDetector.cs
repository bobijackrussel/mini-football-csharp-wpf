using Mini_football.Core.Entities;
using Mini_football.Core.Field;

namespace Mini_football.Core.Services;

public enum GoalOwner
{
    Player1,
    Player2
}

public class GoalDetector
{
    private readonly GoalDefinition _player1Goal;
    private readonly GoalDefinition _player2Goal;
    private readonly double _ballWidth;
    private readonly double _threshold;

    public GoalDetector(GoalDefinition player1Goal, GoalDefinition player2Goal, double ballWidth)
    {
        _player1Goal = player1Goal;
        _player2Goal = player2Goal;
        _ballWidth = ballWidth;
        _threshold = _ballWidth * 0.75;
    }

    public GoalOwner? DetectGoal(BallState ball)
    {
        var ballLeft = ball.Point.X + _ballWidth / 2;
        var ballTop = ball.Point.Y + ball.Element.RenderSize.Height / 2;

        if (IsInsideGoalArea(ballLeft, ballTop, _player1Goal))
        {
            return GoalOwner.Player1;
        }

        if (IsInsideGoalArea(ballLeft, ballTop, _player2Goal))
        {
            return GoalOwner.Player2;
        }

        return null;
    }

    private bool IsInsideGoalArea(double ballLeft, double ballTop, GoalDefinition goal)
    {
        var linePosition = goal.LinePosition + (goal.IsLeftGoal ? _threshold : -_threshold);
        var withinVerticalBounds = ballTop >= goal.TopPostPosition && ballTop <= goal.BottomPostPosition;

        if (!withinVerticalBounds)
        {
            return false;
        }

        if (goal.IsLeftGoal)
        {
            return ballLeft <= linePosition;
        }

        return ballLeft >= linePosition;
    }
}
