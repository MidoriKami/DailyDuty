using System;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Features.MiniCactpot;

public unsafe class MiniCactpot : Module<ConfigBase, Data> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Mini Cactpot",
        FileName = "MiniCactpot",
        Type = ModuleType.Daily,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "DoH", "DoL", "Exp" ],
    };

    public override DataNodeBase DataNode => new DataNode(this);

    protected override void OnEnable() {
         Services.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
    }

    protected override void OnDisable() {
         Services.AddonLifecycle.UnregisterListener(LotteryDailyPreSetup);
    }

    protected override StatusMessage GetStatusMessage()
        => $"{ModuleData.AllowancesRemaining} Attempts Remaining";

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.AllowancesRemaining is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;
    
    private void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo) {
        ModuleData.AllowancesRemaining -= 1;
        ModuleData.MarkDirty();
    }

    public override void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1010445) return;

        if (sceneDataCount is 5) {
            ModuleData.AllowancesRemaining = (int) sceneData[4];
        }
        else {
            ModuleData.AllowancesRemaining = 0;
        }

        ModuleData.MarkDirty();
    }
}
