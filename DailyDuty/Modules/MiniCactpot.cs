using DailyDuty.Classes;
using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;

namespace DailyDuty.Modules;

public class MiniCactpotData : ModuleData {
    public int AllowancesRemaining = 3;
    
    protected override void DrawModuleData() {
        DrawDataTable([
            (Strings.AllowancesRemaining, AllowancesRemaining.ToString()),
        ]);
    }
}

public class MiniCactpotConfig : ModuleConfig {
    public bool ClickableLink = true;
    
    protected override void DrawModuleConfig() {
        ConfigChanged |= ImGui.Checkbox(Strings.ClickableLink, ref ClickableLink);
    }
}

public unsafe class MiniCactpot : BaseModules.Modules.Daily<MiniCactpotData, MiniCactpotConfig>, IGoldSaucerMessageReceiver {
    public override ModuleName ModuleName => ModuleName.MiniCactpot;
    
    public override bool HasClickableLink => Config.ClickableLink;
    
    public override PayloadId ClickableLinkPayloadId => PayloadId.GoldSaucerTeleport;

    public override void Load() {
        base.Load();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
    }

    public override void Unload() {
        base.Unload();
        
        Service.AddonLifecycle.UnregisterListener(LotteryDailyPreSetup);
    }

    public override void Reset() {
        Data.AllowancesRemaining = 3;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() 
        => Data.AllowancesRemaining == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
        LinkEnabled = Config.ClickableLink,
        Message = $"{Data.AllowancesRemaining} {Strings.TicketsRemaining}",
        Payload = PayloadId.GoldSaucerTeleport,
    };

    private void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo) {
        Data.AllowancesRemaining -= 1;
        DataChanged = true;
    }

    public void GoldSaucerUpdate(GoldSaucerEventArgs data) {
        const int miniCactpotBroker = 1010445;
        if (Service.TargetManager.Target?.DataId is not miniCactpotBroker) return;

        if (data.EventId == 5) {
            Data.AllowancesRemaining = data.Data[4];
            DataChanged = true;
        }
        else {
            Data.AllowancesRemaining = 0;
            DataChanged = true;
        } 
    }
}