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
    private Func<T, bool> filter;
    private readonly IEnumerable<T> luminaRows;
    private readonly BaseModule module;
    
    public LuminaTaskUpdater(BaseModule module, Func<T, bool> filter)
    {
        this.filter = filter;
        this.module = module;

        luminaRows = LuminaCache<T>.Instance.Where(filter);
    }

    public void UpdateConfig(List<LuminaTaskConfig> configValues)
    {
        if (configValues.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!configValues.Any(task => task.RowId == luminaEntry.RowId))
                {
                    configValues.Add(new LuminaTaskConfig
                    {
                        RowId = luminaEntry.RowId,
                        Enabled = false,
                    });
                }
            }
            
            module.SaveConfig();
        }
    }

    public void UpdateData(List<LuminaTaskData> dataValues)
    {
        if (dataValues.Count != luminaRows.Count())
        {
            foreach (var luminaEntry in luminaRows)
            {
                if (!dataValues.Any(task => task.RowId == luminaEntry.RowId))
                {
                    dataValues.Add(new LuminaTaskData
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