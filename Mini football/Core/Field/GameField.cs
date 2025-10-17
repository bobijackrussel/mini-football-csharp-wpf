using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mini_football.Core.Field;

public class GameField
{
    public GameField(Canvas canvas, IEnumerable<FrameworkElement> obstacles)
    {
        Canvas = canvas;
        Obstacles = obstacles.ToList();
    }

    public Canvas Canvas { get; }

    public IReadOnlyList<FrameworkElement> Obstacles { get; }

    public Rect Bounds => new(0, 0, Canvas.ActualWidth > 0 ? Canvas.ActualWidth : Canvas.Width, Canvas.ActualHeight > 0 ? Canvas.ActualHeight : Canvas.Height);
}
