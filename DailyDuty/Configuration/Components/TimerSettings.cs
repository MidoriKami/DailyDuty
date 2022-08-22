using System.Numerics;
using DailyDuty.Configuration.Enums;
using DailyDuty.Utilities;

namespace DailyDuty.Configuration.Components;

public class TimerSettings
{
    public readonly Setting<TimerStyle> TimerStyle = new(Enums.TimerStyle.Full);
    public readonly Setting<Vector4> BackgroundColor = new(Colors.Black);
    public readonly Setting<Vector4> ForegroundColor = new(Colors.Purple);
    public readonly Setting<Vector4> TextColor = new(Colors.White);
    public readonly Setting<Vector4> TimeColor = new(Colors.White);
    public readonly Setting<int> Size = new(200);
    public readonly Setting<bool> StretchToFit = new(false);
}