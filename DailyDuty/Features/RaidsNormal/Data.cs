using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.RaidsNormal;

public class Data : DataBase {
    public Dictionary<uint, bool> TaskStatus = [];
}
