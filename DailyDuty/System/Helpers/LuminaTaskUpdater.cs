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

    public void UpdateConfig(List<LuminaTaskConfig<T>> configValues)
    {
        if (configValues.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!configValues.Any(task => task.RowId == luminaEntry.RowId))
                {
                    configValues.Add(new LuminaTaskConfig<T>
                    {
                        RowId = luminaEntry.RowId,
                        Enabled = false,
                    });
                }
            }
            
            module.SaveConfig();
        }
    }

    public void UpdateData(List<LuminaTaskData<T>> dataValues)
    {
        if (dataValues.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!dataValues.Any(task => task.RowId == luminaEntry.RowId))
                {
                    dataValues.Add(new LuminaTaskData<T>
                    {
                        RowId = luminaEntry.RowId,
                        Complete = false,
                    });
                }
            }
            
            module.SaveData();
        }
    }
}