using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.GrandCompanyProvision;

public class GrandCompanyProvisionConfig : ConfigBase {
    public Dictionary<uint, bool> TrackedClasses = new() {
        [16] = true,
        [17] = true,
        [18] = true,
    };
}
