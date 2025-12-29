using System;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.GrandCompanySquadron;

/// <summary>
/// This module needs to be completely redesigned.
/// </summary>
public class GrandCompanySquadron : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Grand Company Squadron",
        FileName = "GrandCompanySquadron",
        Type = ModuleType.Hidden,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "GrandCompany", "GC", "Gil", "Company Seals", "Seals" ],
    };

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
