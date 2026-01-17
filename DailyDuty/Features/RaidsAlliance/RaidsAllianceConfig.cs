using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsAlliance;

public class RaidsAllianceConfig : ConfigBase {
    public Dictionary<uint, bool> TrackedTasks = [];
}
