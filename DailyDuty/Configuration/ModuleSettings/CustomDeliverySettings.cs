using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.ModuleSettings;

internal class CustomDeliverySettings : GenericSettings
{
    public readonly Setting<int> NotificationThreshold = new(12);
    public readonly Setting<ComparisonMode> ComparisonMode = new(Enums.ComparisonMode.EqualTo);
}