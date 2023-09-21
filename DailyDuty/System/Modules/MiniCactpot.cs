using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using DailyDuty.Models.ModuleData;
using DailyDuty.System.Localization;
using Dalamud.Game.Addon;

namespace DailyDuty.System;

public unsafe class MiniCactpot : Module.DailyModule, IGoldSaucerMessageReceiver
{
    public override IModuleDataBase ModuleData { get; protected set; } = new MiniCactpotData();
    public override IModuleConfigBase ModuleConfig { get; protected set; } = new MiniCactpotConfig();
    private MiniCactpotData Data => ModuleData as MiniCactpotData ?? new MiniCactpotData();
    private MiniCactpotConfig Config => ModuleConfig as MiniCactpotConfig ?? new MiniCactpotConfig()
    ;public override ModuleName ModuleName => ModuleName.MiniCactpot;
    
    public override bool HasClickableLink => true;
    public override PayloadId ClickableLinkPayloadId => PayloadId.GoldSaucerTeleport;

    public override void Load()
    {
        base.Load();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreSetup, "LotteryDaily", LotteryDailyPreSetup);
    }

    public override void Unload()
    {
        base.Unload();
        
        Service.AddonLifecycle.UnregisterListener(LotteryDailyPreSetup);
    }

    public override void Reset()
    {
        Data.AllowancesRemaining = 3;
        
        base.Reset();
    }

    protected override ModuleStatus GetModuleStatus() => Data.AllowancesRemaining == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override StatusMessage GetStatusMessage()
    {
        var message = $"{Data.AllowancesRemaining} {Strings.TicketsRemaining}";

        return ConditionalStatusMessage.GetMessage(Config.ClickableLink, message, PayloadId.GoldSaucerTeleport);
    }

    public void LotteryDailyPreSetup(AddonEvent eventType, AddonArgs addonInfo)
    {
        Data.AllowancesRemaining -= 1;
        DataChanged = true;
    }

    public void GoldSaucerUpdate(object? sender, GoldSaucerEventArgs data)
    {
        const int miniCactpotBroker = 1010445;
        if (Service.TargetManager.Target?.DataId is not miniCactpotBroker) return;

        if (data.EventId == 5)
        {
            Data.AllowancesRemaining = data.Data[4];
            DataChanged = true;
        }
        else
        {
            Data.AllowancesRemaining = 0;
            DataChanged = true;
        } 
    }
}