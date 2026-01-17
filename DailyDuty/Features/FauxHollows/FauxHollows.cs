using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;

namespace DailyDuty.Features.FauxHollows;

public class FauxHollows : Module<FauxHollowsConfig, FauxHollowsData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Faux Hollows",
        FileName = "FauxHollows",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Poetics" ],
    };

    public override DataNodeBase DataNode => new FauxHollowsDataNode(this);
    public override ConfigNodeBase ConfigNode => new FauxHollowsConfigNode(this);

    protected override void OnModuleEnable()
        => Services.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "WeeklyPuzzle", WeeklyPuzzlePreSetup);

    protected override void OnModuleDisable()
        => Services.AddonLifecycle.UnregisterListener(WeeklyPuzzlePreSetup);

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset()
        => ModuleData.FauxHollowsCompletions = 0;

    protected override StatusMessage GetStatusMessage() => new() {
        Message = "Unreal Trial Available",
        PayloadId = PayloadId.IdyllshireTeleport,
    };

    public override DateTime GetNextResetDateTime() 
        => Time.NextWeeklyReset();

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.IncludeRetelling switch {
        true when ModuleData.FauxHollowsCompletions is 2 => CompletionStatus.Complete,
        false when ModuleData.FauxHollowsCompletions is 1 => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };
    
    private void WeeklyPuzzlePreSetup(AddonEvent type, AddonArgs args) {
        ModuleData.FauxHollowsCompletions++;
        ModuleData.MarkDirty();
    }
}
