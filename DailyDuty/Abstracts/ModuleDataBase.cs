using System;

namespace DailyDuty.Abstracts;

public abstract class ModuleDataBase
{
    public DateTime NextReset { get; set; }
}