using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Configuration.Enums;

[Flags]
public enum WindowAnchor
{
    TopLeft = 0,
    TopRight = 1,
    BottomLeft = 2,
    BottomRight = 1 | 2
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