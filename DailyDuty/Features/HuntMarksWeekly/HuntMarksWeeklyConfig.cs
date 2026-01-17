using System.Collections.Generic;
using DailyDuty.Classes;

namespace DailyDuty.Features.HuntMarksWeekly;

public class HuntMarksWeeklyConfig : ConfigBase {
    public List<uint> TrackedHuntMarks = [];
}
