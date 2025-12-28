using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.DomanEnclave;

public unsafe class DomanEnclave : Module<ConfigBase, DomanEnclaveData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Doman Enclave",
        FileName = "DomanEnclave",
        Type = ModuleType.Weekly,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Money", "Gil" ],
        MessageClickAction = PayloadId.DomanEnclaveTeleport,
    };

    public override DataNodeBase DataNode => new DataNode(this);

    protected override ReadOnlySeString GetStatusMessage() 
        => ModuleStatus is CompletionStatus.Unknown ? "Status unknown, visit the enclave to update" : $"{RemainingAllowance:N0} gil remaining";

    public override DateTime GetNextResetDateTime() 
        => Time.NextWeeklyReset();

    public override void Reset() {
        ModuleData.DonatedThisWeek = 0;
        ModuleData.RemainingAllowance = ModuleData.WeeklyAllowance;
    }

    protected override CompletionStatus GetCompletionStatus() {
        if (DomanEnclaveManager.Instance()->State.Allowance is 0) return CompletionStatus.Unknown;
        return RemainingAllowance is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    protected override void Update() {
        base.Update();

        var allowance = DomanEnclaveManager.Instance()->State.Allowance;
        var donated = DomanEnclaveManager.Instance()->State.Donated; 
        
        if (allowance is 0) return;
        
        if (ModuleData.WeeklyAllowance != allowance) {
            ModuleData.WeeklyAllowance = allowance;
            ModuleData.SavePending = true;
        }

        if (ModuleData.DonatedThisWeek != donated) {
            ModuleData.DonatedThisWeek = donated;
            ModuleData.SavePending = true;
        }
    }

    private int RemainingAllowance => ModuleData.WeeklyAllowance - ModuleData.DonatedThisWeek;
}
