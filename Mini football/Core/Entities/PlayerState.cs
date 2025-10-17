using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mini_football.Core.Input;
using Mini_football.Models;

namespace Mini_football.Core.Entities;

public class PlayerState
{
    public PlayerState(FrameworkElement element, ControlScheme controls)
    {
        Element = element;
        Controls = controls;
        Input = new PlayerInputState();
        SpawnPosition = new Point(Canvas.GetLeft(element), Canvas.GetTop(element));
    }

    public FrameworkElement Element { get; }

    public ControlScheme Controls { get; }

    public PlayerInputState Input { get; }

    public Point SpawnPosition { get; }

    public double Speed { get; set; } = 5.0;

    public Rect Bounds => new(GetLeft(), GetTop(), Element.RenderSize.Width, Element.RenderSize.Height);

    public Point Center => new(Point.X + Element.RenderSize.Width / 2, Point.Y + Element.RenderSize.Height / 2);

    public Point Point => new(GetLeft(), GetTop());

    public void ResetPosition()
    {
        Canvas.SetLeft(Element, SpawnPosition.X);
        Canvas.SetTop(Element, SpawnPosition.Y);
        Input.Reset();
    }

    public void SetPosition(double left, double top)
    {
        Canvas.SetLeft(Element, left);
        Canvas.SetTop(Element, top);
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
