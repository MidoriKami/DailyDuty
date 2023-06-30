using DailyDuty.Abstracts;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Models.ModuleData;

[Category("ModuleData", 1)]
public interface IDutyRouletteModuleData
{
    [IntDisplay("CurrentWeeklyTomestones")]
    public int ExpertTomestones { get; set; }

    [IntDisplay("WeeklyTomestoneLimit")]
    public int ExpertTomestoneCap { get; set; }

    [BoolDisplay("AtWeeklyTomestoneLimit")]
    public bool AtTomeCap { get; set; }
}

public class DutyRouletteData : ModuleTaskDataBase<ContentRoulette>, IDutyRouletteModuleData
{
    public int ExpertTomestones { get; set; }
    public int ExpertTomestoneCap { get; set; }
    public bool AtTomeCap { get; set; }
}