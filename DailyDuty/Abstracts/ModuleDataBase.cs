using System;
using DailyDuty.Models.Attributes;

namespace DailyDuty.Abstracts;

public class ModuleDataBase
{
    [ModuleResetTime("ModuleReset", 0)]
    public DateTime NextReset = DateTime.MinValue;
}