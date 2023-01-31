using DailyDuty.Localization;
using Dalamud.Utility;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.DataModels;

public record TrackedContentNote(int RowID, Setting<bool> Enabled, bool Completed) : IInfoBoxListConfigurationRow, IInfoBoxTableDataRow
{
    public string GetTaskName() => LuminaCache<ContentsNote>.Instance
        .GetRow((uint) RowID)?.Name.ToDalamudString().ToString() ?? "Unable to Read Name";

    public void GetConfigurationRow(InfoBoxList owner)
    {
        owner.AddConfigCheckbox(GetTaskName(), Enabled);
    }
    
    public unsafe void GetDataRow(InfoBoxTable owner)
    {
        var status = FFXIVClientStructs.FFXIV.Client.Game.UI.ContentsNote.Instance()->IsContentNoteComplete(RowID);
        
        owner
            .BeginRow()
            .AddString(GetTaskName())
            .AddString(status ? Strings.Common_Complete : Strings.Common_Incomplete, status ? Colors.Green : Colors.SoftRed)
            .EndRow();
    }
    
    public bool Completed { get; set; } = Completed;
}
