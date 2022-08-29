using System.Collections.Generic;
using System;
using System.Numerics;
using System.Text;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

[Flags]
public enum TimerStyle
{
    Days = 0b1000,
    Hours = 0b0100,
    Minutes = 0b0010,
    Seconds = 0b0001,

    Full = Days | Hours | Minutes | Seconds,
    DaysNoSeconds = Days | Hours | Minutes,
    NoDays = Hours | Minutes | Seconds,
    NoDaysNoSeconds = Hours | Minutes,
}

public class TimerSettings
{
    public Setting<TimerStyle> TimerStyle = new(Components.TimerStyle.Full);
    public Setting<Vector4> BackgroundColor = new(Colors.Black);
    public Setting<Vector4> ForegroundColor = new(Colors.Purple);
    public Setting<Vector4> TextColor = new(Colors.White);
    public Setting<Vector4> TimeColor = new(Colors.White);
    public Setting<int> Size = new(200);
    public Setting<bool> StretchToFit = new(false);
    public Setting<bool> UseCustomName = new(false);
    public Setting<string> CustomName = new(string.Empty);
    public Setting<bool> HideLabel = new(false);
    public Setting<bool> HideTime = new(false);
}

public static class TimerStyleExtensions
{
    public static string GetLabel(this TimerStyle style)
    {
        if (style == 0)
            return string.Empty;

        var sb = new StringBuilder(16);
        if (style.HasFlag(TimerStyle.Days)) sb.Append($"D").Append('.');
        if (style.HasFlag(TimerStyle.Hours)) sb.Append($"HH").Append(':');
        if (style.HasFlag(TimerStyle.Minutes)) sb.Append($"MM").Append(':');
        if (style.HasFlag(TimerStyle.Seconds)) sb.Append($"SS").Append(':');

        return sb.ToString(0, sb.Length - 1);
    }

    public static IEnumerable<TimerStyle> GetConfigurableStyles()
    {
        return new List<TimerStyle>()
        {
            TimerStyle.Full,
            TimerStyle.DaysNoSeconds,
            TimerStyle.NoDays,
            TimerStyle.NoDaysNoSeconds
        };
    }
}