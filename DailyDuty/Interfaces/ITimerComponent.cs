using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Windows;
using KamiLib;
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

public class TimersConfigurationRow : IInfoBoxTableConfigurationRow
{
    private readonly ITimerComponent timerComponent;
    public TimersConfigurationRow(ITimerComponent component)
    {
        timerComponent = component;
    }

    public void GetConfigurationRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddConfigCheckbox(timerComponent.ParentModule.Name.GetTranslatedString(), timerComponent.ParentModule.GenericSettings.TimerTaskEnabled)
            .AddButton(Strings.Timers_EditStyle + $"##{timerComponent.ParentModule.Name}",() => KamiCommon.WindowManager.ToggleWindowOfType<TimerStyleConfigurationWindow>())
            .EndRow();
    }
}