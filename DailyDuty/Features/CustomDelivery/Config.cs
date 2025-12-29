using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.CustomDelivery;

public class Config : ConfigBase {
    public int NotificationThreshold = 0;
    public ComparisonMode ComparisonMode = ComparisonMode.Above;
}
