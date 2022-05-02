using System;
using DailyDuty.Localization;

namespace DailyDuty.Enums
{
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
        public static string GetLabel(this WindowAnchor anchor)
        {
            return anchor switch
            {
                WindowAnchor.TopLeft => Strings.Common.TopLeftLabel,
                WindowAnchor.TopRight => Strings.Common.TopRightLabel,
                WindowAnchor.BottomLeft => Strings.Common.BottomLeftLabel,
                WindowAnchor.BottomRight => Strings.Common.BottomRightLabel,
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
        }
    }
}
