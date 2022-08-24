using System.Numerics;
using DailyDuty.Configuration.Enums;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

public class TimerSettings
{
    public Setting<TimerStyle> TimerStyle = new(Enums.TimerStyle.Full);
    public Setting<Vector4> BackgroundColor = new(Colors.Black);
    public Setting<Vector4> ForegroundColor = new(Colors.Purple);
    public Setting<Vector4> TextColor = new(Colors.White);
    public Setting<Vector4> TimeColor = new(Colors.White);
    public Setting<int> Size = new(200);
    public Setting<bool> StretchToFit = new(false);
}