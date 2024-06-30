using DailyDuty.Classes;
using DailyDuty.Models;
using Lumina.Excel.GeneratedSheets;
using DailyDuty.Localization;
using DailyDuty.Modules.BaseModules;
using ClientStructs = FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.Modules;

public unsafe class ChallengeLog : BaseModules.Modules.WeeklyTask<ModuleTaskData<ContentsNote>, ModuleTaskConfig<ContentsNote>, ContentsNote> {
    public override ModuleName ModuleName => ModuleName.ChallengeLog;
    
    public override bool HasClickableLink => true;
    
    public override PayloadId ClickableLinkPayloadId => PayloadId.OpenChallengeLog;

    protected override void UpdateTaskLists() {
        var luminaUpdater = new LuminaTaskUpdater<ContentsNote>(this, row => row.RequiredAmount is not 0);
        luminaUpdater.UpdateConfig(Config.TaskConfig);
        luminaUpdater.UpdateData(Data.TaskData);
    }

    public override void Update() {
        Data.TaskData.Update(ref DataChanged, rowId => ClientStructs.ContentsNote.Instance()->IsContentNoteComplete((int) rowId));

        base.Update();
    }

    public override void Reset() {
        Data.TaskData.Reset();
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus()
        => IncompleteTaskCount is 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{IncompleteTaskCount} {Strings.TasksIncomplete}",
    };
}