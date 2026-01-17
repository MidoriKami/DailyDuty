using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.Levequest;

public class LevequestConfig : ConfigBase {
    public int NotificationThreshold = 95;
    public ComparisonMode ComparisonMode = ComparisonMode.Above;
}
