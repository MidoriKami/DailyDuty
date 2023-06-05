using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models;
using KamiLib.Caching;
using Lumina.Excel;

namespace DailyDuty.System.Helpers;

public class LuminaTaskUpdater<T> where T : ExcelRow
{
    private readonly IEnumerable<T> luminaRows;
    private readonly BaseModule module; 

    public LuminaTaskUpdater(BaseModule module, Func<T, bool> filter)
    {
        this.module = module;

        luminaRows = LuminaCache<T>.Instance.Where(filter);
    }

    public void UpdateConfig(LuminaTaskConfigList<T> configValues)
    {
        if (configValues.ConfigList.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!configValues.ConfigList.Any(task => task.RowId == luminaEntry.RowId))
                {
                    configValues.ConfigList.Add(new LuminaTaskConfig<T>
                    {
                        RowId = luminaEntry.RowId,
                        Enabled = false,
                        TargetCount = 0
                    });
                }
            }
            
            module.SaveConfig();
        }
    }
    
    public void UpdateData(LuminaTaskDataList<T> dataList)
    {
        if (dataList.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!dataList.Any(task => task.RowId == luminaEntry.RowId))
                {
                    dataList.Add(new LuminaTaskData<T>
                    {
                        RowId = luminaEntry.RowId,
                        Complete = false,
                        CurrentCount = 0
                    });
                }
            }
            
            module.SaveData();
        }
    }
}