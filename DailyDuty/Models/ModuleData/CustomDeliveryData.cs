using System;
using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface ICustomDeliveryModuleData
{
    [IntDisplay("AllowancesRemaining")]
    public int RemainingAllowances { get; set; }
}

public class CustomDeliveryData : IModuleDataBase, ICustomDeliveryModuleData
{
    public int RemainingAllowances { get; set; } = 12;
    public DateTime NextReset { get; set; } = DateTime.MinValue;
}