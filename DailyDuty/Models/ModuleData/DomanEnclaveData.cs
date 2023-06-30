using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IDomanEnclaveModuleData
{
    [IntDisplay("WeeklyAllowance")]
    public int WeeklyAllowance { get; set; }

    [IntDisplay("DonatedThisWeek")]
    public int DonatedThisWeek { get; set; }
    
    [IntDisplay("BudgetRemaining")]
    public int RemainingAllowance { get; set; }
}

public class DomanEnclaveData : IModuleDataBase, IDomanEnclaveModuleData
{
    public int WeeklyAllowance { get; set; }
    public int DonatedThisWeek { get; set; }
    public int RemainingAllowance { get; set; }
    public DateTime NextReset { get; set; } = DateTime.MinValue;
}