using System;

namespace DailyDuty.Data.SettingsObjects;

[Serializable]
public class SystemSettings
{
    public int ZoneChangeDelayRate = 1;
    public bool ShowSaveDebugInfo = false;
    public bool ClickableLinks = false;
    public bool EnableDebugOutput = false;
}