using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.SampleModule;

// Template SampleModification for more easily creating your own, can copy this entire folder and rename it.
public class SampleModule : Module<ConfigBase, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "SampleModule",
        FileName = "SampleModule",
        Type = ModuleType.Hidden,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Tag" ],
        MessageClickAction = PayloadId.Unset, // Remove or change, don't leave as unset.
    };

    public override DataNodeBase GetDataNode() 
        => new DataNode(this);

    public override ConfigNodeBase GetConfigNode() 
        => new ConfigNode(this);

    protected override void OnEnable() { }

    protected override void OnDisable() { }

    protected override ReadOnlySeString GetStatusMessage() 
        => "ModuleStatus";

    public override DateTime GetNextResetDateTime() 
        => Time.NextDailyReset();

    public override void Reset() { }

    protected override CompletionStatus GetCompletionStatus()
        => CompletionStatus.Unknown;
}
