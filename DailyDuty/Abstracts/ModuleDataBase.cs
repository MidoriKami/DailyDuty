using System;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Abstracts;

public class ModuleDataBase
{
    [DrawCategory("ModuleReset", 0)]
    [ModuleResetTime]
    public DateTime NextReset { get; set; }
}