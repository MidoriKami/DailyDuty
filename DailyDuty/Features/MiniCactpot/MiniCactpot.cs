using DailyDuty.Utilities;
using System;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace DailyDuty.Features.MiniCactpot;

public class MiniCactpot : Module<ConfigBase, MiniCactpotData> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = Strings.Mini_Cactpot,
        FileName = "MiniCactpot",
        Type = ModuleType.Daily,
        Tags = ["DoH", "DoL", "Exp"],
    };

    public override DataNodeBase DataNode => new MiniCactpotDataNode(this);

    protected override async Task OnModuleEnable() {
        await Services.Framework.Run(() => {
            Services.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
        });
    }

    protected override async Task OnModuleDisable() {
        await Services.Framework.Run(() => {
            Services.AddonLifecycle.UnregisterListener(LotteryDailyPreSetup);
        });
    }

    protected override StatusMessage GetStatusMessage() => new() {
        Message = $"{ModuleData.AllowancesRemaining} {Strings.Ticket_s__Remaining}",
        PayloadId = PayloadId.GoldSaucerTeleport,
    };

    public override DateTime GetNextResetDateTime()
        => Time.NextDailyReset();

    public override TimeSpan GetResetPeriod()
        => TimeSpan.FromDays(1);

    public override void Reset() {
        ModuleData.AllowancesRemaining = 3;
    }

    protected override CompletionStatus GetCompletionStatus()
        => ModuleData.AllowancesRemaining is 0 ? CompletionStatus.Complete : CompletionStatus.Incomplete;

    private void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo) {
        ModuleData.AllowancesRemaining -= 1;
        ModuleData.MarkDirty();
    }

    public override unsafe void OnNpcInteract(EventFramework* thisPtr, GameObject* gameObject, EventId eventId, short scene, ulong sceneFlags, uint* sceneData, byte sceneDataCount) {
        if (gameObject->BaseId is not 1010445) return;

        if (sceneDataCount is 5) {
            ModuleData.AllowancesRemaining = (int)sceneData[4];
        }
        else {
            ModuleData.AllowancesRemaining = 0;
        }

        ModuleData.MarkDirty();
    }
}
