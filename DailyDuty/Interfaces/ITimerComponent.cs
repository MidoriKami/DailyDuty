using System;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Windows;
using KamiLib;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface ITimerComponent : IInfoBoxTableConfigurationRow
{
    IModule ParentModule { get; }
    TimeSpan GetTimerPeriod();
    DateTime GetNextReset();
    TimeSpan RemainingTime => GetNextReset() - DateTime.UtcNow;

    void IInfoBoxTableConfigurationRow.GetConfigurationRow(InfoBoxTable owner)
    { 
        owner
            .BeginRow()
            .AddConfigCheckbox(ParentModule.Name.GetTranslatedString(), ParentModule.GenericSettings.TimerTaskEnabled)
            .AddButton(Strings.Timers_EditStyle + $"##{ParentModule.Name}",() => KamiCommon.WindowManager.AddWindow(new TimersStyleWindow(ParentModule)))
            .EndRow();
    }
}