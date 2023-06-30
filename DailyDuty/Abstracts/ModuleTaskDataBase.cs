using System;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using KamiLib.AutomaticUserInterface;
using Lumina.Excel;

namespace DailyDuty.Abstracts;

[Category("TaskData", 3)]
public class ModuleTaskDataBase<T> : IModuleDataBase where T : ExcelRow
{
    [DataList] 
    public LuminaTaskDataList<T> TaskData = new();

    public DateTime NextReset { get; set; } = DateTime.MinValue;
}