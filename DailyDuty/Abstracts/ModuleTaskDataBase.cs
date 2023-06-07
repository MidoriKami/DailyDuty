using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

public class ModuleTaskDataBase<T> : ModuleDataBase where T : ExcelRow
{
    [DataList("TaskData", 3)] 
    public LuminaTaskDataList<T> TaskData = new();
}