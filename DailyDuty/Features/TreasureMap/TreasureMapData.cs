using System;
using DailyDuty.Classes;

namespace DailyDuty.Features.TreasureMap;

public class TreasureMapData : DataBase {
    public DateTime LastMapGatheredTime = DateTime.MinValue;
}
