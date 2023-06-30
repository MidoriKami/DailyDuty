using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IFashionReportModuleData
{
    [IntDisplay("AllowancesRemaining")]
    public int AllowancesRemaining { get; set; }
    
    [IntDisplay("HighestWeeklyScore")]
    public int HighestWeeklyScore { get; set; }

    [BoolDisplay("FashionReportAvailable")]
    public bool FashionReportAvailable { get; set; }
}

public class FashionReportData : IModuleDataBase, IFashionReportModuleData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    // Module Data
    public int AllowancesRemaining { get; set; } = 4;
    public int HighestWeeklyScore { get; set; }
    public bool FashionReportAvailable { get; set; }
}