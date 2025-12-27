using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.DomanEnclave;

public unsafe class DomanEnclave : Module<ConfigBase, DataBase> {
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

    public override DataNodeBase GetDataNode() 
        => new DataNode(this);

    public override ConfigNodeBase GetConfigNode() 
        => new ConfigNode(this);

    protected override void OnEnable() { }

    protected override void OnDisable() { }

    protected override ReadOnlySeString GetStatusMessage() 
        => ModuleStatus is CompletionStatus.Unknown ? "Status unknown, visit the enclave to update" : $"{RemainingAllowance:N0} gil remaining";

    public override DateTime GetNextResetDateTime() 
        => Time.NextWeeklyReset();

    public override void Reset() { }

    protected override CompletionStatus GetCompletionStatus() {
        if (Allowance is 0) return CompletionStatus.Unknown;
        
        return RemainingAllowance is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    }

    private static int Allowance => DomanEnclaveManager.Instance()->State.Allowance;
    private static int Donated => DomanEnclaveManager.Instance()->State.Donated;
    private static int RemainingAllowance => Allowance - Donated;
}
