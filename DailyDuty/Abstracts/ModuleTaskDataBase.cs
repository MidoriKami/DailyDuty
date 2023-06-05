using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public class ModuleTaskDataBase<T> : ModuleDataBase where T : ExcelRow
{
    [DrawCategory("TaskData", 3)]
    [DataList] 
    public LuminaTaskDataList<T> TaskData = new();
}