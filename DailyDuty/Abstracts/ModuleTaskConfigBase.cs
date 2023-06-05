using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public class ModuleTaskConfigBase<T> : ModuleConfigBase where T : ExcelRow
{
    [DrawCategory("TaskSelection", 3)]
    [ConfigList]
    public LuminaTaskConfigList<T> TaskConfig = new();
}