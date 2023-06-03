using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public class ModuleTaskDataBase<T> : ModuleDataBase where T : ExcelRow
{
    [SelectableTasks] 
    public LuminaTaskDataList<T> TaskData = new();
}