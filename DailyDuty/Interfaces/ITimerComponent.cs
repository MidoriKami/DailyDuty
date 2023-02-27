using System;
using DailyDuty.DataModels;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface ITimerComponent
{
    IModule ParentModule { get; }
    TimeSpan GetTimerPeriod();
    DateTime GetNextReset();
    TimeSpan RemainingTime => GetNextReset() - DateTime.UtcNow;

    TimersConfigurationRow GetTimersConfigurationRow() => new(this);
}

public class TimersConfigurationRow : IInfoBoxListConfigurationRow
{
    private readonly ITimerComponent timerComponent;
    public TimersConfigurationRow(ITimerComponent component)
    {
        timerComponent = component;
    }
    
    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner
            .AddConfigCheckbox(timerComponent.ParentModule.Name.GetTranslatedString(), timerComponent.ParentModule.GenericSettings.TimerTaskEnabled);
    }
}