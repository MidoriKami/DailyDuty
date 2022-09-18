using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Lumina.Excel.GeneratedSheets;

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

public record TrackedRaid(DutyInformation Duty, Setting<bool> Tracked, Setting<int> NumItems, uint CurrentDropCount = 0) : IInfoBoxTableConfigurationRow, IInfoBoxTableDataRow
{
    public ModuleStatus GetStatus()
    {
        return CurrentDropCount >= NumItems.Value ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    public uint CurrentDropCount { get; set; } = CurrentDropCount;

    public void GetConfigurationRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddConfigCheckbox(Duty.Name, Tracked)
            .AddInputInt(Strings.Module.Raids.Drops + $"##{Duty.Name}", NumItems, 0, 10, 0, 0, 30.0f)
            .EndRow();
    }

    public void GetDataRow(InfoBoxTable owner)
    {
        owner
            .BeginRow()
            .AddString(Duty.Name)
            .AddString($"{CurrentDropCount} / {NumItems}", CurrentDropCount >= NumItems.Value ? Colors.Green : Colors.Orange)
            .EndRow();
    }
}