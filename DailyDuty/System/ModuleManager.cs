using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Modules;
using DailyDuty.Modules.Enums;
using DailyDuty.Utilities;

namespace DailyDuty.System;

internal class ModuleManager : IDisposable
{
    private List<IModule> Modules { get; } = new()
    {
        new BeastTribe(),
        new CustomDelivery(),
        new DomanEnclave()
    };

    public ModuleManager()
    {
        Log.Verbose("Constructing ModuleManager");
    }

    public void Dispose()
    {
        foreach (var module in Modules)
        {
            module.Dispose();
        }
    }

    public IEnumerable<ISelectable> GetConfigurationSelectables()
    {
        return Modules
            .Select(module => module.ConfigurationComponent.Selectable);
    }

    public IEnumerable<ISelectable> GetStatusSelectables()
    {
        return Modules
            .Select(module => module.StatusComponent.Selectable);
    }

    public IEnumerable<ITodoComponent> GetTodoComponents(CompletionType type)
    {
        return Modules
            .Where(module => module.TodoComponent.CompletionType == type)
            .Select(module => module.TodoComponent);
    }

    public IEnumerable<ITimerComponent> GetTimerComponents()
    {
        return Modules
            .Select(module => module.TimerComponent);
    }

    public IEnumerable<ILogicComponent> GetLogicComponents()
    {
        return Modules
            .Select(module => module.LogicComponent);
    }
}