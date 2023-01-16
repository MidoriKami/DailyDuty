using DailyDuty.Localization;
using Dalamud.Utility;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;

namespace DailyDuty.DataModels;

public record DutyInformation
{
    public uint TerritoryType { get; }
    public uint ContentFinderCondition { get; }
    public string Name { get; }
    
    public DutyInformation(ContentFinderCondition cfc)
    {
        TerritoryType = cfc.TerritoryType.Row;
        ContentFinderCondition = cfc.RowId;
        Name = cfc.Name.ToDalamudString().TextValue;
    }

    [JsonConstructor]
    public DutyInformation(uint territoryType, uint contentFinderCondition, string name)
    {
        TerritoryType = territoryType;
        ContentFinderCondition = contentFinderCondition;
        Name = name;
    }
}

public record TrackedRaid : IInfoBoxTableConfigurationRow, IInfoBoxTableDataRow
{
    public DutyInformation Duty { get; }
    public uint CurrentDropCount { get; set; }
    public Setting<bool> Tracked { get; }
    public Setting<int> NumItems { get; }

    public TrackedRaid(ContentFinderCondition cfc, bool tracked = true, int numItems = 1, uint currentDropCount = 0)
    {
        Duty = new DutyInformation(cfc);
        Tracked = new Setting<bool>(tracked);
        NumItems = new Setting<int>(numItems);
        CurrentDropCount = currentDropCount;
    }

    [JsonConstructor]
    public TrackedRaid(DutyInformation duty, uint currentDropCount, Setting<bool> tracked, Setting<int> numItems)
    {
        Duty = duty;
        CurrentDropCount = currentDropCount;
        Tracked = tracked;
        NumItems = numItems;
    }
    
    public ModuleStatus GetStatus()
    {
        return CurrentDropCount >= NumItems.Value ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

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