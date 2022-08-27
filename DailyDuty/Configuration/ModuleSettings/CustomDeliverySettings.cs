using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.ModuleSettings;

internal class CustomDeliverySettings : GenericSettings
{
    public Setting<int> NotificationThreshold = new(12);
    public Setting<ComparisonMode> ComparisonMode = new(Enums.ComparisonMode.EqualTo);
}