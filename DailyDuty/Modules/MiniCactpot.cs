using System;
using DailyDuty.Addons;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.System;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Utility.Signatures;
using KamiLib.Configuration;
using KamiLib.InfoBoxSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

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

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.Instance.DrawGenericSettings(this);

            InfoBox.Instance
                .AddTitle(Strings.Module.MiniCactpot.ClickableLinkLabel)
                .AddString(Strings.Module.MiniCactpot.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable =>
            new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            if (ParentModule.LogicComponent is not ModuleLogicComponent logicModule) return;

            var moduleStatus = logicModule.GetModuleStatus();

            InfoBox.Instance
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
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }

        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink.Value;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = TeleportManager.Instance.GetPayload(TeleportLocation.GoldSaucer);
            
            SignatureHelper.Initialise(this);

            Service.AddonManager.Get<LotteryDailyAddon>().Show += OnShow;
            Service.AddonManager.Get<GoldSaucerAddon>().GoldSaucerUpdate += OnGoldSaucerUpdate;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<LotteryDailyAddon>().Show -= OnShow;
            Service.AddonManager.Get<GoldSaucerAddon>().GoldSaucerUpdate -= OnGoldSaucerUpdate;
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