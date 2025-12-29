using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.TribalQuests;

public class Config : ConfigBase {
    public int NotificationThreshold = 95;
    public ComparisonMode ComparisonMode = ComparisonMode.Above;
}
