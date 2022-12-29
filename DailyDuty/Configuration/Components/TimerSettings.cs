using System.Collections.Generic;
using System;
using System.Numerics;
using DailyDuty.Utilities;
using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.Utilities;

namespace DailyDuty.Configuration.Components;

[Flags]
public enum TimerStyle
{
    Labelled = 0b10000,
    Days = 0b01000,
    Hours = 0b00100,
    Minutes = 0b00010,
    Seconds = 0b00001,

    Human = Labelled | Days | Hours | Minutes | Seconds,
    Full = Days | Hours | Minutes | Seconds,
    NoSeconds = Days | Hours | Minutes,
}

public class TimerSettings
{
    public Setting<TimerStyle> TimerStyle = new(Components.TimerStyle.Human);
    public Setting<Vector4> BackgroundColor = new(Colors.Black);
    public Setting<Vector4> ForegroundColor = new(Colors.Purple);
    public Setting<Vector4> TextColor = new(Colors.White);
    public Setting<Vector4> TimeColor = new(Colors.White);
    public Setting<int> Size = new(200);
    public Setting<bool> StretchToFit = new(true);
    public Setting<bool> UseCustomName = new(false);
    public Setting<string> CustomName = new(string.Empty);
    public Setting<bool> HideLabel = new(false);
    public Setting<bool> HideTime = new(false);
}

public static class TimerStyleExtensions
{
    public static string GetLabel(this TimerStyle style)
    {
        return style switch
        {
            TimerStyle.Human => Strings.UserInterface.Timers.HumanStyle,
            TimerStyle.Full => Strings.UserInterface.Timers.FullStyle,
            TimerStyle.NoSeconds => Strings.UserInterface.Timers.NoSecondsStyle,
            _ => string.Empty
        };
    }

    public static IEnumerable<TimerStyle> GetConfigurableStyles()
    {
        return new List<TimerStyle>
        {
            TimerStyle.Human,
            TimerStyle.Full,
            TimerStyle.NoSeconds,
        };
    }
}
