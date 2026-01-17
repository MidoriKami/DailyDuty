using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsAlliance;

public class RaidsAllianceData : DataBase {
    public Dictionary<uint, bool> TaskStatus = [];
}
