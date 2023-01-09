using DailyDuty.Localization;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DataModels;

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
            .AddInputInt(Strings.Common_Drops + $"##{Duty.Name}", NumItems, 0, 10, 0, 0, 30.0f)
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