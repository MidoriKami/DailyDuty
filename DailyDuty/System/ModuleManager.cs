using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Configuration;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Modules;
using DailyDuty.Modules.Special;

namespace DailyDuty.System;

internal class ModuleManager : IDisposable
{
    private List<IModule> Modules { get; } = new()
    {
        new BeastTribe(),
        new CustomDelivery(),
        new DomanEnclave(),
        new DutyRoulette(),
        new FashionReport(),
        new HuntMarksDaily(),
        new HuntMarksWeekly(),
        new JumboCactpot(),
        new Levequest(),
        new MiniCactpot(),
        new TreasureMap(),
        new WondrousTails(),
    };

    private readonly IModule dailyTimer = new DailyTimerModule();
    private readonly IModule weeklyTimer = new WeeklyTimerModule();

    public ModuleManager()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable += OverrideTimersEnabled;
    }

    private void OverrideTimersEnabled(object? sender, CharacterConfiguration e)
    {
        dailyTimer.GenericSettings.Enabled.Value = true;
        weeklyTimer.GenericSettings.Enabled.Value = true;

        Service.ConfigurationManager.Save();
    }

    public void Dispose()
    {
        foreach (var module in Modules)
        {
            module.Dispose();
        }

        Service.ConfigurationManager.OnCharacterDataAvailable -= OverrideTimersEnabled;
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
        var moduleComponents = Modules.Select(module => module.TimerComponent).ToList();

        moduleComponents.Add(dailyTimer.TimerComponent);
        moduleComponents.Add(weeklyTimer.TimerComponent);

        return moduleComponents;
    }

    public IEnumerable<ILogicComponent> GetLogicComponents()
    {
        return Modules
            .Select(module => module.LogicComponent);
    }
}