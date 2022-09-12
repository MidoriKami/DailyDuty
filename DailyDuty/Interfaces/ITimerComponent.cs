using System;
using DailyDuty.Configuration.Enums;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;

namespace DailyDuty.Interfaces;

internal interface ITimerComponent : IInfoBoxTableConfigurationRow
{
    IModule ParentModule { get; }
    TimeSpan GetTimerPeriod();
    DateTime GetNextReset();


    void IInfoBoxTableConfigurationRow.GetConfigurationRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddConfigCheckbox(ParentModule.Name.GetTranslatedString(), ParentModule.GenericSettings.TimerTaskEnabled)
            .AddButton(Strings.UserInterface.Timers.EditTimer, () => Service.WindowManager.AddTimerStyleWindow(ParentModule, ParentModule.GenericSettings.TimerSettings))
            .EndRow();
    }
}