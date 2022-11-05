using System;
using DailyDuty.Configuration.Components;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;

namespace DailyDuty.Interfaces;

public interface ITimerComponent : IInfoBoxTableConfigurationRow
{
    IModule ParentModule { get; }
    TimeSpan GetTimerPeriod();
    DateTime GetNextReset();

    void IInfoBoxTableConfigurationRow.GetConfigurationRow(InfoBoxTable owner)
    { 
        owner
            .BeginRow()
            .AddConfigCheckbox(ParentModule.Name.GetTranslatedString(), ParentModule.GenericSettings.TimerTaskEnabled)
            .AddButton(Strings.UserInterface.Timers.EditTimer + $"##{ParentModule.Name}", 
                () => Service.WindowManager.AddWindow(new TimersStyleWindow(ParentModule)))
            .EndRow();
    }
}