using System;
using KamiLib.Configuration;

namespace DailyDuty.DataModels;

public class GenericSettings
{
    public DateTime NextReset = new();

    public Setting<bool> Enabled = new(false);
    public Setting<bool> NotifyOnLogin = new(true);
    public Setting<bool> NotifyOnZoneChange = new(true);

    public Setting<bool> TodoTaskEnabled = new(true);
    public Setting<bool> TodoUseLongLabel = new(false);

    public Setting<bool> TimerTaskEnabled = new(false);
    public TimerSettings TimerSettings = new();
}
