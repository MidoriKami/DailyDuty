using System;
using DailyDuty.Interfaces;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace DailyDuty.Configuration.Components;

public enum RouletteType
{
    Expert = 5,
    Level90 = 8,
    Level50607080 = 2,
    Leveling = 1,
    Trials = 6,
    MSQ = 3,
    Guildhest = 4,
    Alliance = 15,
    Normal = 17,
    Mentor = 9,
    Frontline = 7
}

public static class RouletteTypeExtensions
{
    public static string GetLocalizedString(this RouletteType type)
    {
        return Service.DataManager.GetExcelSheet<ContentRoulette>()!.GetRow((uint) type)!.Category.RawString;
    }
}

public record TrackedRoulette(RouletteType Roulette, Setting<bool> Tracked, bool Completed) : IInfoBoxTableRow
{
    public Tuple<Action?, Action?> GetInfoBoxTableRow()
    {
        return new Tuple<Action?, Action?>(
            Actions.GetStringAction(Roulette.GetLocalizedString()),
            Actions.GetStringAction(Completed ? Strings.Common.Complete : Strings.Common.Incomplete, Completed ? Colors.Green : Colors.Red)
            );
    }

    public bool Completed { get; set; } = Completed;
}
