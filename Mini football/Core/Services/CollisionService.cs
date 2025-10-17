using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mini_football.Core.Entities;

namespace Mini_football.Core.Services;

public static class CollisionService
{
    public static void ResolvePlayerCollisions(PlayerState player1, PlayerState player2)
    {
        var distance = (player2.Center - player1.Center).Length;
        var collisionDistance = (player1.Element.RenderSize.Width + player2.Element.RenderSize.Width) / 2;

        if (distance >= collisionDistance || distance == 0)
        {
            return;
        }

        var direction = player2.Center - player1.Center;
        direction.Normalize();

        player1.SetPosition(player1.Point.X - direction.X * player1.Speed, player1.Point.Y - direction.Y * player1.Speed);
        player2.SetPosition(player2.Point.X + direction.X * player2.Speed, player2.Point.Y + direction.Y * player2.Speed);
    }

    public static void ResolveObstacleCollisions(PlayerState player, IEnumerable<FrameworkElement> obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            PreventOverlap(player.Element, obstacle);
        }
    }

    public static void ResolveObstacleCollisions(BallState ball, IEnumerable<FrameworkElement> obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            if (!IsIntersecting(ball.Element, obstacle))
            {
                continue;
            }

            var collisionDirection = new Vector(
                ball.Point.X + ball.Element.RenderSize.Width / 2 - (Canvas.GetLeft(obstacle) + obstacle.RenderSize.Width / 2),
                ball.Point.Y + ball.Element.RenderSize.Height / 2 - (Canvas.GetTop(obstacle) + obstacle.RenderSize.Height / 2));

            if (collisionDirection.Length > 0)
            {
                collisionDirection.Normalize();
            }

            if (obstacle.Name.Contains("frontPost", StringComparison.OrdinalIgnoreCase))
            {
                ball.Velocity = collisionDirection * Math.Max(ball.Velocity.Length, 5);
            }
            else
            {
                ball.Velocity = new Vector(0, 0);
            }

            const double pushOutDistance = 5;
            ball.SetPosition(ball.Point.X + collisionDirection.X * pushOutDistance, ball.Point.Y + collisionDirection.Y * pushOutDistance);
        }
    }

    public static void ResolveBallPlayerCollision(BallState ball, PlayerState player)
    {
        if (!IsIntersecting(ball.Element, player.Element))
        {
            return;
        }

        var collisionDirection = ball.Center - player.Center;
        if (collisionDirection.Length <= 0)
        {
            return;
        }

        collisionDirection.Normalize();
        ball.Velocity = collisionDirection * Math.Max(ball.Velocity.Length, 5);
    }

    public static bool IsBallNearPlayer(BallState ball, PlayerState player, double distanceThreshold = 50)
    {
        var distance = (ball.Center - player.Center).Length;
        return distance < distanceThreshold;
    }

    private static bool IsIntersecting(FrameworkElement movingObject, FrameworkElement obstacle)
    {
        var movingLeft = Canvas.GetLeft(movingObject);
        var movingTop = Canvas.GetTop(movingObject);
        var obstacleLeft = Canvas.GetLeft(obstacle);
        var obstacleTop = Canvas.GetTop(obstacle);

        if (double.IsNaN(movingLeft) || double.IsNaN(movingTop) || double.IsNaN(obstacleLeft) || double.IsNaN(obstacleTop))
        {
            return false;
        }

        var movingRect = new Rect(movingLeft, movingTop, movingObject.RenderSize.Width, movingObject.RenderSize.Height);
        var obstacleRect = new Rect(obstacleLeft, obstacleTop, obstacle.RenderSize.Width, obstacle.RenderSize.Height);
        return movingRect.IntersectsWith(obstacleRect);
    }

    private static void PreventOverlap(FrameworkElement movingObject, FrameworkElement obstacle)
    {
        if (!IsIntersecting(movingObject, obstacle))
        {
            return;
        }

        var movingLeft = Canvas.GetLeft(movingObject);
        var movingTop = Canvas.GetTop(movingObject);
        var obstacleLeft = Canvas.GetLeft(obstacle);
        var obstacleTop = Canvas.GetTop(obstacle);

        var movingCenterX = movingLeft + movingObject.RenderSize.Width / 2;
        var movingCenterY = movingTop + movingObject.RenderSize.Height / 2;
        var obstacleCenterX = obstacleLeft + obstacle.RenderSize.Width / 2;
        var obstacleCenterY = obstacleTop + obstacle.RenderSize.Height / 2;

        var overlapX = movingObject.RenderSize.Width / 2 + obstacle.RenderSize.Width / 2 - Math.Abs(movingCenterX - obstacleCenterX);
        var overlapY = movingObject.RenderSize.Height / 2 + obstacle.RenderSize.Height / 2 - Math.Abs(movingCenterY - obstacleCenterY);

        if (overlapX < overlapY)
        {
            if (movingCenterX < obstacleCenterX)
            {
                Canvas.SetLeft(movingObject, movingLeft - overlapX);
            }
            else
            {
                Canvas.SetLeft(movingObject, movingLeft + overlapX);
            }
        }
        else
        {
            if (movingCenterY < obstacleCenterY)
            {
                Canvas.SetTop(movingObject, movingTop - overlapY);
            }
            else
            {
                Canvas.SetTop(movingObject, movingTop + overlapY);
            }
        }
    }
}
