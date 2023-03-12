using System;

namespace DailyDuty.Models.Settings;

[Serializable]
public class SystemSettings
{
    public MessageSettings MessageSettings = new();
}