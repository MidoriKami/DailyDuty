using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface ILevequestModuleData
{
    [IntDisplay("LevequestAllowances")]
    public int NumLevequestAllowances { get; set; }

    [IntDisplay("AcceptedLevequests")] 
    public int AcceptedLevequests { get; set; }
}

public class LevequestData : IModuleDataBase, ILevequestModuleData
{
    public DateTime NextReset { get; set; } = DateTime.MinValue;
    
    public int NumLevequestAllowances { get; set; }

    public int AcceptedLevequests { get; set; }
}
