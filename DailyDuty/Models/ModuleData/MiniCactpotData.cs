using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IMiniCactpotModuleData
{
    [IntDisplay("TicketsRemaining")]
    public int AllowancesRemaining { get; set; }
}

public class MiniCactpotData : IModuleDataBase, IMiniCactpotModuleData
{
    public DateTime NextReset { get; set; } =DateTime.MinValue;
    
    public int AllowancesRemaining { get; set; } = 3;
}
