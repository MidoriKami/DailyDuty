using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.TribalQuests;

public class TribalQuestsConfig : ConfigBase {
    public int NotificationThreshold = 95;
    public ComparisonMode ComparisonMode = ComparisonMode.Above;
}
