using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.GrandCompanySquadron;

/// <summary>
/// This module needs to be completely redesigned.
/// </summary>
public class GrandCompanySquadron : Module<ConfigBase, GrandCompanySquadronData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Grand Company Squadron",
        FileName = "GrandCompanySquadron",
        Type = ModuleType.Hidden,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "GrandCompany", "GC", "Gil", "Company Seals", "Seals" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);
    public override ConfigNodeBase ConfigNode => new ConfigNode(this);

    protected override void OnEnable() {
        throw new Exception("This module should not be loaded.");
    }
    
    protected override void OnDisable() {
    }

    protected override ReadOnlySeString GetStatusMessage()
        => "INVALID MODULE ACTIVATED";

    public override DateTime GetNextResetDateTime()
        => Time.NextGrandCompanyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    public override void Reset() {
    }

    protected override CompletionStatus GetCompletionStatus()
        => CompletionStatus.Disabled;
}
