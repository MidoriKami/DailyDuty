using System;
using DailyDuty.Localization;

namespace DailyDuty.Enums
{
    public enum TodoWindowStyle
    {
        AutoResize,
        ManualSize
    }

    public static class TodoWindowStylesExtensions
    {
        public static string GetLabel(this TodoWindowStyle style)
        {
            return style switch
            {
                TodoWindowStyle.AutoResize => Strings.Common.AutoResizeLabel,
                TodoWindowStyle.ManualSize => Strings.Common.ManualSizeLabel,
                _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
            };
        }
    }
}
