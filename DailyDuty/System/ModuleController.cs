using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models.Modules;
using Dalamud.Logging;

namespace DailyDuty.System;

public class ModuleController : IDisposable
{
    private readonly List<BaseModule> modules;

    public ModuleController()
    {
        modules = new List<BaseModule>
        {
            new TestModule(),
        };
    }
    
    public void Dispose()
    {
        foreach (var module in modules.OfType<IDisposable>())
        {
            module.Dispose();
        }
    }

    public void UpdateModules()
    {
        foreach (var module in modules)
        {
            module.Update();
        }
    }

    public void LoadModules()
    {
        foreach (var module in modules)
        {
            module.Load();
        }
    }
    public void UnloadModules()
    {
        foreach (var module in modules)
        {
            module.Unload();
        }
    }

    public void ResetModules()
    {
        foreach (var module in modules)
        {
            if (DateTime.UtcNow >= module.Data.NextReset)
            {
                PluginLog.Debug($"Resetting module: {module.ModuleName} Next Reset: {module.GetNextReset().ToLocalTime()}");
                
                module.Reset();
                module.Save();
            }
        }
    }

    public void ZoneChange(uint newZone)
    {
        foreach (var module in modules)
        {
            module.ZoneChange(newZone);
        }
    }
}