using System;
using System.Collections.Generic;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IJumboCactpotModuleData
{
    [IntListDisplay("ClaimedTickets")]
    public List<int> Tickets { get; set; }
}

public class JumboCactpotData : IModuleDataBase, IJumboCactpotModuleData
{
    public List<int> Tickets { get; set; } = new();
    public DateTime NextReset { get; set; } = DateTime.MinValue;
}