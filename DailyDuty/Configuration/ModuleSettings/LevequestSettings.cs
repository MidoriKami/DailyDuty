using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.ModuleSettings;

public class LevequestSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(95);
    public Setting<ComparisonMode> ComparisonMode = new Setting<ComparisonMode>(Enums.ComparisonMode.EqualTo);
}