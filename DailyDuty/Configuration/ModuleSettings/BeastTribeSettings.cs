using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.ModuleSettings;

public class BeastTribeSettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(Enums.ComparisonMode.EqualTo);
}