using System;
using System.Numerics;
using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.DataModels;

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
    public Setting<Vector4> UnavailableColor = new(Colors.Orange);
}

internal class TodoOverlaySettings
{
    public Setting<bool> ShowDailyTasks = new(true);
    public Setting<bool> ShowWeeklyTasks = new(true);
    public Setting<bool> HideWhenAllTasksComplete = new(true);
    public Setting<bool> HideCompletedTasks = new(true);
    public Setting<bool> HideUnavailableTasks = new(true);
    public Setting<bool> ShowCategoryAsComplete = new(false);
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
    public static string GetTranslatedString(this WindowAnchor anchor)
    {
        return anchor switch
        {
            WindowAnchor.TopLeft => Strings.Common_TopLeft,
            WindowAnchor.TopRight => Strings.Common_TopRight,
            WindowAnchor.BottomLeft => Strings.Common_BottomLeft,
            WindowAnchor.BottomRight => Strings.Common_BottomRight,
            _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
        };
    }
}