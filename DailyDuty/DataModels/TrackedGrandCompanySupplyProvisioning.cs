using DailyDuty.Localization;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DataModels;

public record TrackedGrandCompanySupplyProvisioning(uint ClassJobID, Setting<bool> Tracked, bool State) : IInfoBoxTableDataRow, IInfoBoxListConfigurationRow
{
    public void GetDataRow(InfoBoxTable owner)
    {
        var jobName = LuminaCache<ClassJob>.Instance.GetRow(ClassJobID)!.Name.RawString;

        owner
            .BeginRow()
            .AddString($"{char.ToUpper(jobName[0]) + jobName[1..]}")
            .AddString(State ? Strings.Common_Complete : Strings.Common_Incomplete, State ? Colors.Green : Colors.Orange)
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        var jobName = LuminaCache<ClassJob>.Instance.GetRow(ClassJobID)!.Name.RawString;

        owner.AddConfigCheckbox($"{char.ToUpper(jobName[0]) + jobName[1..]}", Tracked);
    }

    public bool State { get; set; } = State;
}
