using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class MiniCactpotSettings : GenericSettings
{
    public int TicketsRemaining = 3;
    public Setting<bool> EnableClickableLink = new(false);
}

public unsafe class MiniCactpot : AbstractModule
{
    public override ModuleName Name => ModuleName.MiniCactpot;
    public override CompletionType CompletionType => CompletionType.Daily;

    private static MiniCactpotSettings Settings => Service.ConfigurationManager.CharacterConfiguration.MiniCactpot;
    public override GenericSettings GenericSettings => Settings;

    
    public override DalamudLinkPayload DalamudLinkPayload => TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer);
    public override bool LinkPayloadActive => Settings.EnableClickableLink;
    
    public MiniCactpot()
    {
        SignatureHelper.Initialise(this);

        LotteryDailyAddon.Instance.Show += OnShow;
        GoldSaucerAddon.Instance.GoldSaucerUpdate += OnGoldSaucerUpdate;
    }

    public override void Dispose()
    {
        LotteryDailyAddon.Instance.Show -= OnShow;
        GoldSaucerAddon.Instance.GoldSaucerUpdate -= OnGoldSaucerUpdate;
    }

    private void OnGoldSaucerUpdate(object? sender, GoldSaucerEventArgs e)
    {
        //1010445 Mini Cactpot Broker
        if (Service.TargetManager.Target?.DataId != 1010445) return;

        if (e.EventID == 5)
        {
            Settings.TicketsRemaining = e.Data[4];
            Service.ConfigurationManager.Save();
        }
        else
        {
            Settings.TicketsRemaining = 0;
            Service.ConfigurationManager.Save();
        }
    }

    private void OnShow(object? sender, nint e)
    {
        Settings.TicketsRemaining -= 1;
        Service.ConfigurationManager.Save();
    }

    public override string GetStatusMessage() => $"{Settings.TicketsRemaining} {Strings.MiniCactpot_TicketsRemaining}";
    public override void DoReset() => Settings.TicketsRemaining = 3;
    public override ModuleStatus GetModuleStatus() => Settings.TicketsRemaining == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.GoldSaucer_ClickableLink)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
            .Draw();

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);
            
        InfoBox.Instance
            .AddTitle(Strings.Status_ModuleData)
            .BeginTable()
            .BeginRow()
            .AddString(Strings.MiniCactpot_TicketsRemaining)
            .AddString(Settings.TicketsRemaining.ToString())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}