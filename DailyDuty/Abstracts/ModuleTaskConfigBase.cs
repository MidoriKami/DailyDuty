using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public class ModuleTaskConfigBase<T> : ModuleConfigBase where T : ExcelRow
{
    [ConfigList("TaskSelection", 3)]
    public LuminaTaskConfigList<T> TaskConfig = new();
}