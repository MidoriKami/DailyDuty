using System.Globalization;
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
            .AddString(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jobName))
            .AddString(State ? Strings.Common_Complete : Strings.Common_Incomplete, State ? Colors.Green : Colors.Orange)
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        var jobName = LuminaCache<ClassJob>.Instance.GetRow(ClassJobID)!.Name.RawString;

        owner.AddConfigCheckbox(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jobName), Tracked);
    }

    public bool State { get; set; } = State;
}
