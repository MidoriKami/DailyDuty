using DailyDuty.Configuration.Character.Enums;
using DailyDuty.Configuration.Common;

namespace DailyDuty.Configuration.Character.Components;

public class BeastTribeSettings : GenericSettings
{
    public readonly Setting<int> NotificationThreshold = new(12);
    public readonly Setting<ComparisonMode> ComparisonMode = new(Enums.ComparisonMode.EqualTo);
}