using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Configuration.Components;

public record TrackedGrandCompanySupplyProvisioning(uint ClassJobID, Setting<bool> Tracked, bool State) : IInfoBoxTableDataRow, IInfoBoxListConfigurationRow
{
    public void GetDataRow(InfoBoxTable owner)
    {
        var jobName = Service.DataManager.GetExcelSheet<ClassJob>()!.GetRow(ClassJobID)!.Name.RawString;

        owner
            .BeginRow()
            .AddString($"{char.ToUpper(jobName[0]) + jobName[1..]}")
            .AddString(State ? Strings.Common.Complete : Strings.Common.Incomplete, State ? Colors.Green : Colors.Orange)
            .EndRow();
    }

    public void GetConfigurationRow(InfoBoxList owner)
    {
        var jobName = Service.DataManager.GetExcelSheet<ClassJob>()!.GetRow(ClassJobID)!.Name.RawString;

        owner.AddConfigCheckbox($"{char.ToUpper(jobName[0]) + jobName[1..]}", Tracked);
    }

    public bool State { get; set; } = State;
}
