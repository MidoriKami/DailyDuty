using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings.Special;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;

namespace DailyDuty.Modules.Special;

internal class DailyTimerModule : IModule
{
    public ModuleName Name => ModuleName.Daily;
    public IConfigurationComponent ConfigurationComponent => throw new InvalidOperationException();
    public IStatusComponent StatusComponent => throw new InvalidOperationException();
    public ILogicComponent LogicComponent => throw new InvalidOperationException();
    public ITodoComponent TodoComponent => throw new InvalidOperationException();
    public ITimerComponent TimerComponent { get; }

    private static DailySettings Settings => Service.ConfigurationManager.CharacterConfiguration.Daily;
    public GenericSettings GenericSettings => Settings;

    public DailyTimerModule()
    {
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {

    }

    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}   