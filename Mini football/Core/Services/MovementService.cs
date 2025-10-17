using System;
using System.Windows;
using Mini_football.Core.Entities;

namespace Mini_football.Core.Services;

public static class MovementService
{
    public static void UpdatePlayerPosition(PlayerState player, Rect bounds)
    {
        var dx = 0.0;
        var dy = 0.0;

        if (player.Input.Up)
        {
            dy -= player.Speed;
        }

        if (player.Input.Down)
        {
            dy += player.Speed;
        }

        if (player.Input.Left)
        {
            dx -= player.Speed;
        }

        if (player.Input.Right)
        {
            dx += player.Speed;
        }

        if (Math.Abs(dx) < double.Epsilon && Math.Abs(dy) < double.Epsilon)
        {
            return;
        }

        var newLeft = Math.Clamp(player.Point.X + dx, bounds.Left, bounds.Right - player.Element.RenderSize.Width);
        var newTop = Math.Clamp(player.Point.Y + dy, bounds.Top, bounds.Bottom - player.Element.RenderSize.Height);

        player.SetPosition(newLeft, newTop);
    }

    public static void UpdateBallPosition(BallState ball, Rect bounds)
    {
        var newLeft = ball.Point.X + ball.Velocity.X;
        var newTop = ball.Point.Y + ball.Velocity.Y;

        if (newLeft < bounds.Left)
        {
            newLeft = bounds.Left;
            ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
        }
        else if (newLeft > bounds.Right - ball.Element.RenderSize.Width)
        {
            newLeft = bounds.Right - ball.Element.RenderSize.Width;
            ball.Velocity = new Vector(-ball.Velocity.X, ball.Velocity.Y);
        }

        if (newTop < bounds.Top)
        {
            newTop = bounds.Top;
            ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
        }
        else if (newTop > bounds.Bottom - ball.Element.RenderSize.Height)
        {
            newTop = bounds.Bottom - ball.Element.RenderSize.Height;
            ball.Velocity = new Vector(ball.Velocity.X, -ball.Velocity.Y);
        }

        ball.SetPosition(newLeft, newTop);
    }
}
