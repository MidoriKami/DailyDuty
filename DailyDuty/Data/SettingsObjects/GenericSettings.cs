using System;

namespace DailyDuty.Data.SettingsObjects;

public class GenericSettings
{
    public DateTime NextReset = new();
    public bool Enabled = false;
    public bool ZoneChangeReminder = false;
    public bool LoginReminder = false;
}