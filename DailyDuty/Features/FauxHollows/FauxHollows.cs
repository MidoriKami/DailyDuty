using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.FauxHollows;

// Template SampleModification for more easily creating your own, can copy this entire folder and rename it.
public class FauxHollows : Module<FauxHollowsConfig, FauxHollowsData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Faux Hollows",
        FileName = "FauxHollows",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Poetics" ],
        MessageClickAction = PayloadId.IdyllshireTeleport, 
    };

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnEnable()
        => Services.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "WeeklyPuzzle", WeeklyPuzzlePreSetup);

    protected override void OnDisable()
        => Services.AddonLifecycle.UnregisterListener(WeeklyPuzzlePreSetup);

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset() {
        ModuleData.FauxHollowsCompletions = 0;
        ModuleData.SavePending = true;
    }

    protected override ReadOnlySeString GetStatusMessage() 
        => "Unreal Trial Available";

    public override DateTime GetNextResetDateTime() 
        => Time.NextWeeklyReset();

    protected override CompletionStatus GetCompletionStatus() => ModuleConfig.IncludeRetelling switch {
        true when ModuleData.FauxHollowsCompletions is 2 => CompletionStatus.Complete,
        false when ModuleData.FauxHollowsCompletions is 1 => CompletionStatus.Complete,
        _ => CompletionStatus.Incomplete,
    };
    
    private void WeeklyPuzzlePreSetup(AddonEvent type, AddonArgs args) {
        ModuleData.FauxHollowsCompletions++;
        ModuleData.SavePending = true;
    }
}
