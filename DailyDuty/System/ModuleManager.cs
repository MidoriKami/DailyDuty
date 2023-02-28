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
            new TribalQuests(),
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
        
#if DEBUG
        Modules.Add(new TestModule());
#endif
    }

    public void Dispose()
    {
        foreach (var module in Modules)
        {
            module.Dispose();
        }
    }

    public IEnumerable<ISelectable> GetConfigurationSelectables(CompletionType? type = null) => Modules
        .Where(module => type is null || module.TodoComponent.CompletionType == type)
        .Select(module => module.ConfigurationComponent.Selectable)
        .OrderBy(module => module.ID);

    public IEnumerable<ISelectable> GetStatusSelectables() => Modules
        .Select(module => module.StatusComponent.Selectable)
        .OrderBy(module => module.ID);

    public IEnumerable<ITodoComponent> GetTodoComponents(CompletionType type) => Modules
        .Where(module => module.TodoComponent.CompletionType == type)
        .Select(module => module.TodoComponent);

    public IEnumerable<ITimerComponent> GetTimerComponents() => Modules
        .Select(module => module.TimerComponent);

    public IEnumerable<ILogicComponent> GetLogicComponents() => Modules
        .Select(module => module.LogicComponent);
}