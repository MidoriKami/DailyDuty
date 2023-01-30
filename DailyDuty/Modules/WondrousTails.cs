using DailyDuty.AddonOverlays;
using DailyDuty.DataModels;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.GameState;
using KamiLib.Teleporter;

namespace DailyDuty.Modules;

public class WondrousTailsSettings : GenericSettings
{
    public Setting<bool> InstanceNotifications = new(false);
    public Setting<bool> EnableClickableLink = new(false);
    public Setting<bool> UnclaimedBookWarning = new(true);
    public Setting<bool> OverlayEnabled = new(true);
    public Setting<bool> ResendOnCompletion = new(false);
}

public unsafe class WondrousTails : AbstractModule
{
    public override ModuleName Name => ModuleName.WondrousTails;
    public override CompletionType CompletionType => CompletionType.Weekly;

    private static WondrousTailsSettings Settings => Service.ConfigurationManager.CharacterConfiguration.WondrousTails;
    public override GenericSettings GenericSettings => Settings;

    private readonly DutyRouletteOverlay overlay = new();

    public override DalamudLinkPayload DalamudLinkPayload => WondrousTailsBook.Instance.NeedsNewBook ? idyllshireTeleportPayload : openBookPayload;
    public override bool LinkPayloadActive => Settings.EnableClickableLink;

    private const uint WondrousTailsBookItemID = 2002023;

    private readonly DalamudLinkPayload openBookPayload;
    private readonly DalamudLinkPayload idyllshireTeleportPayload;

    private readonly WondrousTailsOverlay wondrousTailsOverlay = new();

    private bool dutyCompleted;
    private uint lastTerritoryType;
    
    public WondrousTails()
    {
        openBookPayload = ChatPayloadManager.Instance.AddChatLink(ChatPayloads.OpenWondrousTails, OpenWondrousTailsBook);
        idyllshireTeleportPayload = TeleportManager.Instance.GetPayload(TeleportLocation.Idyllshire);

        DutyState.Instance.DutyStarted += OnDutyStarted;
        DutyState.Instance.DutyCompleted += OnDutyCompleted;
            
        Service.ClientState.TerritoryChanged += OnZoneChange;
    }

    public override void Dispose()
    {
        DutyState.Instance.DutyStarted -= OnDutyStarted;
        DutyState.Instance.DutyCompleted -= OnDutyCompleted;
            
        Service.ClientState.TerritoryChanged -= OnZoneChange;

        wondrousTailsOverlay.Dispose();
        overlay.Dispose();
    }

    private void OnDutyStarted(uint territory)
    {
        if (!Settings.InstanceNotifications) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!WondrousTailsBook.PlayerHasBook) return;

        var node = WondrousTailsBook.Instance.GetTaskForDuty(territory);
        if (node == null) return;

        var buttonState = node.TaskState;
        
        switch (buttonState)
        {
            case ButtonState.Unavailable when WondrousTailsBook.Instance.Stickers > 0:
                Chat.Print(Strings.WondrousTails_Label, Strings.WondrousTails_RerollNotice);
                Chat.Print(Strings.WondrousTails_Label, Strings.WondrousTails_RerollCount.Format(WondrousTailsBook.Instance.SecondChance), Settings.EnableClickableLink ? DalamudLinkPayload : null);
                break;

            case ButtonState.AvailableNow:
                Chat.Print(Strings.WondrousTails_Label, Strings.WondrousTails_AlreadyAvailable, Settings.EnableClickableLink ? DalamudLinkPayload : null);
                break;

            case ButtonState.Completable:
                Chat.Print(Strings.WondrousTails_Label, Strings.WondrousTails_Completable);
                break;

            case ButtonState.Unknown:
            default:
                break;
        }
    }

    private void OnDutyCompleted(uint territory)
    {
        if (!Settings.InstanceNotifications) return;
        if (GetModuleStatus() == ModuleStatus.Complete) return;
        if (!WondrousTailsBook.PlayerHasBook) return;

        dutyCompleted = true;
        lastTerritoryType = territory;

        var node = WondrousTailsBook.Instance.GetTaskForDuty(territory);

        var buttonState = node?.TaskState;

        if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
        {
            Chat.Print(Strings.WondrousTails_Label, Strings.WondrousTails_Claimable, Settings.EnableClickableLink ? DalamudLinkPayload : null);
        }
    }

    private void OnZoneChange(object? sender, ushort e)
    {
        if (!Settings.Enabled) return;
        if (!Settings.ResendOnCompletion) return;
        if (!dutyCompleted) return;

        dutyCompleted = false;
        OnDutyCompleted(lastTerritoryType);
    }

    public override string GetStatusMessage()
    {
        if (Condition.IsBoundByDuty()) return string.Empty;
            
        if (Settings.UnclaimedBookWarning && WondrousTailsBook.Instance.NewBookAvailable)
        {
            return Strings.WondrousTails_BookAvailable;
        }

        return string.Empty;
    }

    public override ModuleStatus GetModuleStatus()
    {
        if (Settings.UnclaimedBookWarning && WondrousTailsBook.Instance.NewBookAvailable) return ModuleStatus.Incomplete;

        return WondrousTailsBook.Instance.Complete ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    }

    private void OpenWondrousTailsBook(uint arg1, SeString arg2)
    {
        if (WondrousTailsBook.PlayerHasBook)
        {
            AgentInventoryContext.Instance()->UseItem(WondrousTailsBookItemID);
        }
    }

    protected override void DrawConfiguration()
    {
        InfoBox.Instance
            .AddTitle(Strings.Config_Options)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
            .AddConfigCheckbox(Strings.DutyFinder_Overlay, Settings.OverlayEnabled)
            .AddConfigCheckbox(Strings.WondrousTails_DutyNotifications, Settings.InstanceNotifications)
            .AddIndent(1)
            .AddConfigCheckbox(Strings.WondrousTails_ResendNotification, Settings.ResendOnCompletion)
            .AddIndent(-1)
            .AddConfigCheckbox(Strings.WondrousTails_UnclaimedBookWarning, Settings.UnclaimedBookWarning)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common_ClickableLink)
            .AddString(Strings.WondrousTails_ClickableLink)
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
            .AddString(Strings.Common_Stamps)
            .AddString($"{WondrousTailsBook.Instance.Stickers} / 9", GetModuleStatus().GetStatusColor())
            .EndRow()
            .EndTable()
            .Draw();
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}