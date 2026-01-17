using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsNormal;

public class RaidsNormalData : DataBase {
    public Dictionary<uint, bool> TaskStatus = [];
}
