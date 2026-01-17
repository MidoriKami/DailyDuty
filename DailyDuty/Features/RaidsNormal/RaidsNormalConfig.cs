using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsNormal;

public class RaidsNormalConfig : ConfigBase {
    public Dictionary<uint, bool> TrackedTasks = [];
}
