using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IFauxHollowsModuleData
{
    [IntDisplay("FauxHollowsCompletions")]
    public int FauxHollowsCompletions { get; set; }
}

public class FauxHollowsData : IModuleDataBase, IFauxHollowsModuleData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    public int FauxHollowsCompletions { get; set; }
}