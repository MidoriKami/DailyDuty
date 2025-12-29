using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanyProvision;

public class Data : DataBase {
    public Dictionary<uint, bool> ClassJobStatus = new() {
        [16] = false,
        [17] = false,
        [18] = false,
    };
}
