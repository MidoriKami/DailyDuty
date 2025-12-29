using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.HuntMarksDaily;

public class Config : ConfigBase {
    public List<uint> TrackedHuntMarks = [];
}
