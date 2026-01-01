using System;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DailyDuty.Features.DomanEnclave;

public unsafe class DomanEnclave : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Doman Enclave",
        FileName = "DomanEnclave",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Money", "Gil" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);

    protected override StatusMessage GetStatusMessage() => new() {
        Message = ModuleStatus is CompletionStatus.Unknown ? "Status unknown, visit the enclave to update" : $"{RemainingAllowance:N0} gil Remaining",
        PayloadId = PayloadId.DomanEnclaveTeleport,
    };

    public override DateTime GetNextResetDateTime() 
        => Time.NextWeeklyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(7);

    public override void Reset()
        => ModuleData.DonatedThisWeek = 0;

    protected override CompletionStatus GetCompletionStatus() {
        if (ModuleData.WeeklyAllowance is 0) return CompletionStatus.Unknown;
        return RemainingAllowance is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    protected override void OnModuleUpdate() {
        var allowance = DomanEnclaveManager.Instance()->State.Allowance;
        var donated = DomanEnclaveManager.Instance()->State.Donated; 
        
        if (allowance is 0) return;
        
        if (ModuleData.WeeklyAllowance != allowance) {
            ModuleData.WeeklyAllowance = allowance;
            ModuleData.MarkDirty();
        }

        if (ModuleData.DonatedThisWeek != donated) {
            ModuleData.DonatedThisWeek = donated;
            ModuleData.MarkDirty();
        }
    }

    private int RemainingAllowance => ModuleData.WeeklyAllowance - ModuleData.DonatedThisWeek;
}
