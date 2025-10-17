using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mini_football.Core.Field;

public class GoalDefinition
{
    public GoalDefinition(FrameworkElement goalLine, FrameworkElement topPost, FrameworkElement bottomPost, IEnumerable<FrameworkElement> obstacles, bool isLeftGoal)
    {
        GoalLine = goalLine;
        TopPost = topPost;
        BottomPost = bottomPost;
        Obstacles = obstacles.ToList();
        IsLeftGoal = isLeftGoal;
    }

    public FrameworkElement GoalLine { get; }
    public FrameworkElement TopPost { get; }
    public FrameworkElement BottomPost { get; }
    public IReadOnlyList<FrameworkElement> Obstacles { get; }
    public bool IsLeftGoal { get; }

    public double LinePosition => Canvas.GetLeft(GoalLine);

    public double TopPostPosition => Canvas.GetTop(TopPost);

    public double BottomPostPosition => Canvas.GetTop(BottomPost) + BottomPost.RenderSize.Height;
}
