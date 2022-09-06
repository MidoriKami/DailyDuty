using System;
using DailyDuty.Addons;
using DailyDuty.Addons.Overlays;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.DataStructures;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules;

internal class WondrousTails : IModule
{
    public ModuleName Name => ModuleName.WondrousTails;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static WondrousTailsSettings Settings => Service.ConfigurationManager.CharacterConfiguration.WondrousTails;
    public GenericSettings GenericSettings => Settings;

    private readonly DutyRouletteOverlay overlay = new();

    public WondrousTails()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {
        overlay.Dispose();
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox optionsInfoBox = new();
        private readonly InfoBox notificationOptionsInfoBox = new();
        private readonly InfoBox clickableLink = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            optionsInfoBox
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .AddConfigCheckbox(Strings.Module.WondrousTails.Overlay, Settings.OverlayEnabled)
                .AddConfigCheckbox(Strings.Module.WondrousTails.DutyNotifications, Settings.InstanceNotifications)
                .AddConfigCheckbox(Strings.Module.WondrousTails.UnclaimedBookNotifications, Settings.UnclaimedBookWarning)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.WondrousTails.ClickableLinkLabel)
                .AddString(Strings.Module.WondrousTails.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            notificationOptionsInfoBox
                .AddTitle(Strings.Configuration.NotificationOptions)
                .AddConfigCheckbox(Strings.Configuration.OnLogin, Settings.NotifyOnLogin)
                .AddConfigCheckbox(Strings.Configuration.OnZoneChange, Settings.NotifyOnZoneChange)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        private readonly InfoBox status = new();

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            status
                .AddTitle(Strings.Status.Label)
                .BeginTable()

                .AddRow(
                    Strings.Status.ModuleStatus,
                    moduleStatus.GetTranslatedString(),
                    secondColor: moduleStatus.GetStatusColor())

                .AddRow(
                    Strings.Module.WondrousTails.Stamps,
                    $"{logicModule.WondrousTailsBook.GetNumStickers()} / 9",
                    secondColor: logicModule.GetModuleStatus().GetStatusColor()
                    )
                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }

        public DalamudLinkPayload DalamudLinkPayload => WondrousTailsBook.NewBookAvailable() ? idyllshireTeleportPayload : openBookPayload;

        private delegate void UseItemDelegate(IntPtr a1, uint a2, uint a3 = 9999, uint a4 = 0, short a5 = 0);

        [Signature("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 B0 01 BA 13 00 00 00")]
        private readonly UseItemDelegate useItemFunction = null!;

        private IntPtr ItemContextMenuAgent => (IntPtr)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.InventoryContext);
        private const uint WondrousTailsBookItemID = 2002023;

        private readonly DalamudLinkPayload openBookPayload;
        private readonly DalamudLinkPayload idyllshireTeleportPayload;

        public WondrousTailsBook WondrousTailsBook { get; } = new();
        private readonly WondrousTailsOverlay wondrousTailsOverlay = new();

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            openBookPayload = Service.PayloadManager.AddChatLink(ChatPayloads.OpenWondrousTails, OpenWondrousTailsBook);
            idyllshireTeleportPayload = Service.TeleportManager.GetPayload(TeleportLocation.Idyllshire);

            Service.AddonManager.Get<DutyEventAddon>().DutyStarted += OnDutyStarted;
            Service.AddonManager.Get<DutyEventAddon>().DutyCompleted += OnDutyCompleted;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<DutyEventAddon>().DutyStarted -= OnDutyStarted;
            Service.AddonManager.Get<DutyEventAddon>().DutyCompleted -= OnDutyCompleted;

            wondrousTailsOverlay.Dispose();
        }
        
        public string GetStatusMessage()
        {
            if (Condition.IsBoundByDuty()) return string.Empty;
            
            if (Settings.UnclaimedBookWarning.Value && WondrousTailsBook.NewBookAvailable())
            {
                return Strings.Module.WondrousTails.BookAvailable;
            }

            return string.Empty;
        }

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            // Do nothing
        }

        public ModuleStatus GetModuleStatus()
        {
            if (Settings.UnclaimedBookWarning.Value && WondrousTailsBook.NewBookAvailable()) return ModuleStatus.Incomplete;

            return WondrousTailsBook.GetNumStickers() == 9 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
        }

        private void OpenWondrousTailsBook(uint arg1, SeString arg2)
        {
            if (ItemContextMenuAgent != IntPtr.Zero && WondrousTailsBook.PlayerHasBook())
            {
                useItemFunction(ItemContextMenuAgent, WondrousTailsBookItemID);
            }
        }
        
        private void OnDutyStarted(object? sender, EventArgs args)
        {
            if (!Settings.InstanceNotifications.Value) return;
            if (GetModuleStatus() == ModuleStatus.Complete) return;
            if (!WondrousTailsBook.PlayerHasBook()) return;

            var node = WondrousTailsBook.GetTaskForDuty(Service.ClientState.TerritoryType);
            if (node == null) return;

            var buttonState = node.TaskState;
        
            switch (buttonState)
            {
                case ButtonState.Unavailable when WondrousTailsBook.GetNumStickers() > 0:
                    Chat.Print(Strings.Module.WondrousTails.Label, Strings.Module.WondrousTails.UnavailableMessage);
                    Chat.Print(Strings.Module.WondrousTails.Label, Strings.Module.WondrousTails.UnavailableRerollMessage.Format(WondrousTailsBook.GetNumSecondChance()), Settings.EnableClickableLink.Value ? DalamudLinkPayload : null);
                    break;

                case ButtonState.AvailableNow:
                    Chat.Print(Strings.Module.WondrousTails.Label, Strings.Module.WondrousTails.AvailableMessage, Settings.EnableClickableLink.Value ? DalamudLinkPayload : null);
                    break;

                case ButtonState.Completable:
                    Chat.Print(Strings.Module.WondrousTails.Label, Strings.Module.WondrousTails.CompletableMessage);
                    break;

                case ButtonState.Unknown:
                    break;
            }
        }

        private void OnDutyCompleted(object? sender, EventArgs args)
        {
            if (!Settings.InstanceNotifications.Value) return;
            if (GetModuleStatus() == ModuleStatus.Complete) return;
            if (!WondrousTailsBook.PlayerHasBook()) return;

            var node = WondrousTailsBook.GetTaskForDuty(Service.ClientState.TerritoryType);

            var buttonState = node?.TaskState;

            if (buttonState is ButtonState.Completable or ButtonState.AvailableNow)
            {
                Chat.Print(Strings.Module.WondrousTails.Label, Strings.Module.WondrousTails.ClaimableMessage, Settings.EnableClickableLink.Value ? DalamudLinkPayload : null);
            }
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Weekly;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.WondrousTails.Label;

        public string GetLongTaskLabel() => Strings.Module.WondrousTails.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(7);

        public DateTime GetNextReset() => Time.NextWeeklyReset();
    }
}