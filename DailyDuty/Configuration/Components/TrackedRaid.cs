using System;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace DailyDuty.Configuration.Components;

public record DutyInformation(uint TerritoryType, uint ContentFinderCondition, string Name)
{
    public static DutyInformation Construct(ContentFinderCondition cfc)
    {
        return new DutyInformation(
            cfc.TerritoryType.Row,
            cfc.RowId,
            cfc.Name.RawString);
    }
};

public record TrackedRaid(DutyInformation Duty, Setting<bool> Tracked, Setting<int> NumItems, uint CurrentDropCount = 0) : IInfoBoxTableRow
{
    public Tuple<Action?, Action?> GetConfigurationRow()
    {
        return new Tuple<Action?, Action?>(
            Actions.GetConfigCheckboxAction(Duty.Name, Tracked),
            Actions.GetInputIntAction(Strings.Module.Raids.Drops + $"##{Duty.Name}", NumItems));
    }

    public Tuple<Action?, Action?> GetDataRow()
    {
        return new Tuple<Action?, Action?>(
            Actions.GetStringAction(Duty.Name),
            Actions.GetStringAction($"{CurrentDropCount} / {NumItems}",
                (CurrentDropCount >= NumItems.Value) ? Colors.Green : Colors.Orange));
    }

    public ModuleStatus GetStatus()
    {
        return CurrentDropCount >= NumItems.Value ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    public uint CurrentDropCount { get; set; } = CurrentDropCount;
}