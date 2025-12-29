using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanySupply;

public class Data : DataBase {
    public Dictionary<uint, bool> ClassJobStatus = new() {
        [8] = false,
        [9] = false,
        [10] = false,
        [11] = false,
        [12] = false,
        [13] = false,
        [14] = false,
        [15] = false,
    };
}
