using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Modules;
using DailyDuty.Utilities;

namespace DailyDuty.System;

internal class ModuleManager : IDisposable
{
    private List<IModule> Modules { get; } = new()
    {
        new DebugModule(),
    };

    public ModuleManager()
    {
        Log.Verbose("Constructing ModuleManager");
    }

    public void Dispose()
    {
        
    }

    public IEnumerable<ISelectable> GetConfigurationSelectables()
    {
        return Modules.Select(module => module.ConfigurationComponent.Selectable).ToList();
    }

    public IEnumerable<ISelectable> GetStatusSelectables()
    {
        return Modules.Select(module => module.StatusComponent.Selectable).ToList();
    }
}