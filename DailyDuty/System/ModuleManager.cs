using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Modules;
using KamiLib.Interfaces;

namespace DailyDuty.System;

internal class ModuleManager : IDisposable
{
    private List<IModule> Modules { get; } 

    public ModuleManager()
    {
        Modules = new List<IModule>
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
            new ChallengeLog(),
            new RaidsNormal(),
            new RaidsAlliance(),
            new FauxHollows(),
            new GrandCompanySupply(),
            new GrandCompanyProvision(),
            new MaskedCarnivale(),
            new GrandCompanySquadron(),
        };
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
            .Select(module => module.ConfigurationComponent.Selectable)
            .OrderBy(module => module.ID);
    }

    public IEnumerable<ISelectable> GetStatusSelectables()
    {
        return Modules
            .Select(module => module.StatusComponent.Selectable)
            .OrderBy(module => module.ID);
    }

    public IEnumerable<ITodoComponent> GetTodoComponents(CompletionType type)
    {
        return Modules
            .Where(module => module.TodoComponent.CompletionType == type)
            .Select(module => module.TodoComponent);
    }

    public IEnumerable<ITimerComponent> GetTimerComponents()
    {
        return Modules.Select(module => module.TimerComponent);
    }

    public IEnumerable<ILogicComponent> GetLogicComponents()
    {
        return Modules
            .Select(module => module.LogicComponent);
    }
}