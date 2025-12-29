using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanySupply;

public class Config : ConfigBase {
    public Dictionary<uint, bool> TrackedClasses = new() {
        [8] = true,
        [9] = true,
        [10] = true,
        [11] = true,
        [12] = true,
        [13] = true,
        [14] = true,
        [15] = true,
    };
}
