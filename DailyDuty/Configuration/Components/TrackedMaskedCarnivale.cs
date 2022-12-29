using System;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using KamiLib.Configuration;
using KamiLib.Utilities;

namespace DailyDuty.Configuration.Components;

public enum CarnivaleTask
{
    Novice,
    Moderate,
    Advanced
}

public record TrackedMaskedCarnivale(CarnivaleTask Task, Setting<bool> Tracked, bool State) : IInfoBoxTableDataRow, IInfoBoxListConfigurationRow
{
    public void GetDataRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddString(Task.GetTranslatedString())
            .AddString(State ? Strings.Common.Complete : Strings.Common.Incomplete, State ? Colors.Green : Colors.Orange)
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner.AddConfigCheckbox(Task.GetTranslatedString(), Tracked);
    }

    public bool State { get; set; } = State;
}

public static class CarnivaleTaskExtensions
{
    public static string GetTranslatedString(this CarnivaleTask task)
    {
        return task switch
        {
            CarnivaleTask.Novice => Strings.Module.MaskedCarnivale.Novice,
            CarnivaleTask.Moderate => Strings.Module.MaskedCarnivale.Moderate,
            CarnivaleTask.Advanced => Strings.Module.MaskedCarnivale.Advanced,
            _ => throw new ArgumentOutOfRangeException(nameof(task), task, null)
        };
    }
}