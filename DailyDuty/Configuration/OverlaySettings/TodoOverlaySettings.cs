using DailyDuty.Configuration.Components;
using System;
using System.Numerics;
using DailyDuty.Localization;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.OverlaySettings;

[Flags]
public enum WindowAnchor
{
    TopLeft = 0,
    TopRight = 1,
    BottomLeft = 2,
    BottomRight = 1 | 2
}

public class TaskColors
{
    public Setting<Vector4> HeaderColor = new(Colors.White);
    public Setting<Vector4> IncompleteColor = new(Colors.Red);
    public Setting<Vector4> CompleteColor = new(Colors.Green);
}

internal class TodoOverlaySettings
{
    public Setting<bool> ShowDailyTasks = new(true);
    public Setting<bool> ShowWeeklyTasks = new(true);
    public Setting<bool> HideWhenAllTasksComplete = new(true);
    public Setting<bool> HideCompletedTasks = new(true);
    public Setting<bool> HideUnavailableTasks = new(true);
    public Setting<bool> HideWhileInDuty = new(true);
    public Setting<bool> LockWindowPosition = new(false);
    public Setting<float> Opacity = new(1.0f);
    public Setting<WindowAnchor> AnchorCorner = new(WindowAnchor.TopLeft);
    public Setting<bool> AutoResize = new(true);
    public Setting<bool> Enabled = new(false);

    public readonly TaskColors TaskColors = new();
}

public static class WindowAnchorExtensions
{
    public static string GetLocalizedString(this WindowAnchor anchor)
    {
        return anchor switch
        {
            WindowAnchor.TopLeft => Strings.Common.TopLeft,
            WindowAnchor.TopRight => Strings.Common.TopRight,
            WindowAnchor.BottomLeft => Strings.Common.BottomLeft,
            WindowAnchor.BottomRight => Strings.Common.BottomRight,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
    }
}