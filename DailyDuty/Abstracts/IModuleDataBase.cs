using System;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Abstracts;

[Category("ModuleReset")]
public interface IModuleDataBase
{
    [ModuleResetTime]
    public DateTime NextReset { get; set; }
}