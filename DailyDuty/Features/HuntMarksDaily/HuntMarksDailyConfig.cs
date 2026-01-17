using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.HuntMarksDaily;

public class HuntMarksDailyConfig : ConfigBase {
    public List<uint> TrackedHuntMarks = [];
}
