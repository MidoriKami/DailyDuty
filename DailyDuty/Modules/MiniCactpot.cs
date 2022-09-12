using System;
using DailyDuty.Addons;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;

namespace DailyDuty.Modules;

public class MiniCactpotSettings : GenericSettings
{
    public int TicketsRemaining = 3;
    public Setting<bool> EnableClickableLink = new(false);
}

internal class MiniCactpot : IModule
{
    public ModuleName Name => ModuleName.MiniCactpot;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static MiniCactpotSettings Settings => Service.ConfigurationManager.CharacterConfiguration.MiniCactpot;
    public GenericSettings GenericSettings => Settings;

    public MiniCactpot()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
    }

    public void Dispose()
    {
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox optionsInfoBox = new();
        private readonly InfoBox clickableLink = new();
        private readonly InfoBox notificationOptionsInfoBox = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            optionsInfoBox
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.MiniCactpot.ClickableLinkLabel)
                .AddString(Strings.Module.MiniCactpot.ClickableLink)
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

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

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
                .BeginRow()
                .AddString(Strings.Status.ModuleStatus)
                .AddString(moduleStatus.GetTranslatedString(), moduleStatus.GetStatusColor())
                .EndRow()
                .BeginRow()
                .AddString(Strings.Module.MiniCactpot.TicketsRemaining)
                .AddString(Settings.TicketsRemaining.ToString())
                .EndRow()
                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }

        public DalamudLinkPayload? DalamudLinkPayload { get; }= Service.TeleportManager.GetPayload(TeleportLocation.GoldSaucer);

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            Service.AddonManager.Get<LotteryDailyAddon>().OnShow += OnShow;
            Service.AddonManager.Get<GoldSaucerAddon>().OnGoldSaucerUpdate += OnGoldSaucerUpdate;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<LotteryDailyAddon>().OnShow -= OnShow;
            Service.AddonManager.Get<GoldSaucerAddon>().OnGoldSaucerUpdate -= OnGoldSaucerUpdate;
        }

        public string GetStatusMessage() => $"{Settings.TicketsRemaining} {Strings.Module.MiniCactpot.TicketsRemaining}";

        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset() => Settings.TicketsRemaining = 3;

        public ModuleStatus GetModuleStatus() => Settings.TicketsRemaining == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;

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

        private void OnShow(object? sender, IntPtr e)
        {
            Settings.TicketsRemaining -= 1;
            Service.ConfigurationManager.Save();
        }
    }

    private class ModuleTodoComponent : ITodoComponent
    {
        public IModule ParentModule { get; }
        public CompletionType CompletionType => CompletionType.Daily;
        public bool HasLongLabel => false;

        public ModuleTodoComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public string GetShortTaskLabel() => Strings.Module.MiniCactpot.Label;

        public string GetLongTaskLabel() => Strings.Module.MiniCactpot.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromDays(1);

        public DateTime GetNextReset() => Time.NextDailyReset();
    }
}