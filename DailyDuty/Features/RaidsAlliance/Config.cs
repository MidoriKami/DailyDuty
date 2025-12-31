using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsAlliance;

public class Config : ConfigBase {
    public Dictionary<uint, bool> TrackedTasks = [];
}
