using System;
using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.DataModels;

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
            .AddString(State ? Strings.Common_Complete : Strings.Common_Incomplete, State ? Colors.Green : Colors.Orange)
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
            CarnivaleTask.Novice => Strings.MaskedCarnivale_Novice,
            CarnivaleTask.Moderate => Strings.MaskedCarnivale_Moderate,
            CarnivaleTask.Advanced => Strings.MaskedCarnivale_Advanced,
            _ => throw new ArgumentOutOfRangeException(nameof(task), task, null)
        };
    }
}