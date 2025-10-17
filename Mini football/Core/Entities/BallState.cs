using System.Windows;
using System.Windows.Controls;

namespace Mini_football.Core.Entities;

public class BallState
{
    public BallState(FrameworkElement element)
    {
        Element = element;
        SpawnPosition = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
    }

    public FrameworkElement Element { get; }

    public Point SpawnPosition { get; }

    public Vector Velocity { get; set; } = new(0, 0);

    public double Radius => Element.RenderSize.Width / 2;

    public Point Center => new(Point.X + Radius, Point.Y + Element.RenderSize.Height / 2);

    public Point Point => new(GetLeft(), GetTop());

    public void Reset()
    {
        Canvas.SetLeft(Element, SpawnPosition.X);
        Canvas.SetTop(Element, SpawnPosition.Y);
        Velocity = new Vector(0, 0);
    }

    public void SetPosition(double left, double top)
    {
        Canvas.SetLeft(Element, left);
        Canvas.SetTop(Element, top);
    }

    public void ApplyFriction(double factor)
    {
        Velocity *= factor;
        if (Velocity.Length < 0.05)
        {
            Velocity = new Vector(0, 0);
        }
    }

    public void UpdatePosition()
    {
        SetPosition(GetLeft() + Velocity.X, GetTop() + Velocity.Y);
    }

    private double GetLeft()
    {
        var value = Canvas.GetLeft(Element);
        return double.IsNaN(value) ? 0 : value;
    }

    private double GetTop()
    {
        var value = Canvas.GetTop(Element);
        return double.IsNaN(value) ? 0 : value;
    }
}
