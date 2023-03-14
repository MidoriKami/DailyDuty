using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;

namespace DailyDuty.System;

public class ModuleController : IDisposable
{
    private readonly List<BaseModule> modules;

    public ModuleController()
    {
        modules = new List<BaseModule>();

        foreach (var t in GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseModule))))
        {
            if (t.IsAbstract) continue;
            var module = (BaseModule?) Activator.CreateInstance(t);
            if (module is null) continue;
            
            modules.Add(module);
        }
    }
    
    public void Dispose()
    {
        foreach (var module in modules.OfType<IDisposable>())
        {
            module.Dispose();
        }
    }

    public IEnumerable<BaseModule> GetModules(ModuleType? type = null) => 
        type is null ? modules : modules.Where(module => module.ModuleType == type);

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
        var now = DateTime.UtcNow;
        
        foreach (var module in modules)
        {
            if (now >= module.ModuleData.NextReset)
            {
                module.Reset();
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