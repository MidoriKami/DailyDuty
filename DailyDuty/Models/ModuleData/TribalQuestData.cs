using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface ITribalQuestModuleData
{
    [UintDisplay("AllowancesRemaining")]
    public uint RemainingAllowances { get; set; }
}

public class TribalQuestsData : IModuleDataBase, ITribalQuestModuleData
{
    public DateTime NextReset { get; set; }
    
    public uint RemainingAllowances { get; set; }
}