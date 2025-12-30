using System;
using DailyDuty.Classes;

namespace DailyDuty.Features.TreasureMap;

public class Data : DataBase {
    public DateTime LastMapGatheredTime = DateTime.MinValue;
}
