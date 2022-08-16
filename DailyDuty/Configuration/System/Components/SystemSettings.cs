using System;
using DailyDuty.Configuration.Common;

namespace DailyDuty.Configuration.System.Components;

[Serializable]
public class SystemSettings
{
    public Setting<int> MinutesBetweenThrottledMessages = new(5);
    public Setting<bool> MessageDelay = new(false);
    public Setting<DayOfWeek> DelayDay = new(DayOfWeek.Tuesday);
    public string SelectedLanguage = string.Empty;
}