using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class RaidsAllianceSettings : GenericSettings
{
    public List<TrackedRaid> TrackedRaids = new();
    public Setting<bool> EnableClickableLink = new(true);
}

internal class RaidsAlliance : IModule
{
    public ModuleName Name => ModuleName.AllianceRaids;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static RaidsAllianceSettings Settings => Service.ConfigurationManager.CharacterConfiguration.RaidsAlliance;
    public GenericSettings GenericSettings => Settings;

    public RaidsAlliance()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);

        if (Settings.TrackedRaids.Count == 0)
        {
            RegenerateTrackedRaids();
        }
    }

    internal static void RegenerateTrackedRaids()
    {
        Settings.TrackedRaids.Clear();

        var instanceContents = Service.DataManager.GetExcelSheet<InstanceContent>()!
            .Where(instance => instance.WeekRestriction == 1)
            .Select(instance => instance.RowId);

        var raidDuties = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
            .Where(cfc => instanceContents.Contains(cfc.Content))
            .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse == 8)
            .ToList();

        foreach (var raid in raidDuties)
        {
            var dutyInformation = DutyInformation.Construct(raid);

            Settings.TrackedRaids.Add(new TrackedRaid(dutyInformation, new Setting<bool>(false), new Setting<int>(1)));
        }
    }

    public void Dispose()
    {
        LogicComponent.Dispose();
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(ParentModule, this);

        private readonly InfoBox options = new();
        private readonly InfoBox configuration = new();
        private readonly InfoBox notificationOptions = new();
        private readonly InfoBox regenerateRaids = new();
        private readonly InfoBox clickableLink = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            options
                .AddTitle(Strings.Configuration.Options)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
                .Draw();

            if (Settings.TrackedRaids is { } trackedRaids)
            {
                configuration
                    .AddTitle(Strings.Module.Raids.TrackedNormalRaids)
                    .BeginTable(0.70f)
                    .AddRows(trackedRaids.OfType<IInfoBoxTableConfigurationRow>())
                    .EndTable()
                    .Draw();
            }

            regenerateRaids
                .AddTitle(Strings.Module.Raids.Regenerate)
                .AddString(Strings.Module.Raids.RegenerateHelp, Colors.Orange)
                .AddAction(() =>
                {
                    ImGuiHelpers.ScaledDummy(10.0f);

                    if (ImGui.Button(Strings.Module.Raids.Regenerate))
                    {
                        RegenerateTrackedRaids();
                    }
                })
                .Draw();

            clickableLink
                .AddTitle(Strings.Module.Raids.ClickableLinkLabel)
                .AddString(Strings.Module.Raids.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            notificationOptions
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
        private readonly InfoBox target = new();

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
                .EndTable()
                .Draw();

            if (Settings.TrackedRaids.Any(raid => raid.Tracked.Value))
            {
                target
                    .AddTitle(Strings.Common.Target)
                    .BeginTable(0.70f)
                    .AddRows(Settings.TrackedRaids
                        .Where(raid => raid.Tracked.Value)
                        .OfType<IInfoBoxTableDataRow>())
                    .EndTable()
                    .Draw();
            }
            else
            {
                target
                    .AddTitle(Strings.Common.Target)
                    .AddString(Strings.Module.Raids.NoRaidsTracked, Colors.Orange)
                    .Draw();
            }
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }

        private readonly AgentContentsFinder* contentsFinderAgentInterface = AgentContentsFinder.Instance();

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = Service.PayloadManager.AddChatLink(ChatPayloads.AllianceRaidsDutyFinder, OpenDutyFinder);

            Service.Framework.Update += FrameworkUpdate;
            Service.Chat.ChatMessage += OnChatMessage;
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;
            Service.Chat.ChatMessage -= OnChatMessage;
        }

        private void FrameworkUpdate(Framework framework)
        {
            if(!Settings.Enabled.Value) return;

            var dutyFinderAddon = Service.AddonManager.Get<DutyFinderAddon>();
            if (!dutyFinderAddon.IsOpen) return;

            var enabledRaids = Settings.TrackedRaids.Where(raid => raid.Tracked.Value).ToList();
            if(!enabledRaids.Any()) return;

            var contentFinderCondition = *(int*)((byte*) contentsFinderAgentInterface + 6988);
            var trackedRaid = enabledRaids.FirstOrDefault(raid => raid.Duty.ContentFinderCondition == contentFinderCondition);

            if (trackedRaid != null)
            {
                var numCollectedRewards = *((byte*) contentsFinderAgentInterface + 7000);

                if (trackedRaid.CurrentDropCount != numCollectedRewards)
                {
                    trackedRaid.CurrentDropCount = numCollectedRewards;
                    Service.ConfigurationManager.Save();
                }
            }
        }

        private void OnChatMessage(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            // If module is enabled
            if (!Settings.Enabled.Value) return;

            // If message is a loot message
            if (((int)type & 0x7F) != 0x3E) return;

            // If we are in a zone that we are tracking
            if (GetRaidForCurrentZone() is not { } trackedRaid) return;

            // If the message does NOT contain a player payload
            if (message.Payloads.FirstOrDefault(p => p is PlayerPayload) is PlayerPayload) return;

            // If the message DOES contain an item
            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload) return;

            // We looted an item in a normal raid
            trackedRaid.CurrentDropCount += 1;
            Service.ConfigurationManager.Save();
        }

        
        private void OpenDutyFinder(uint arg1, SeString arg2)
        {
            AgentContentsFinder.Instance()->OpenRegularDuty(GetFirstMissingRaid());
        }

        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Module.Raids.RaidRemaining}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            foreach (var raid in Settings.TrackedRaids)
            {
                raid.CurrentDropCount = 0;
            }
        }

        private int GetIncompleteCount() => Settings.TrackedRaids.Count(raid => raid.Tracked.Value && raid.GetStatus() == ModuleStatus.Incomplete);
        public ModuleStatus GetModuleStatus() => GetIncompleteCount() > 0 ? ModuleStatus.Incomplete : ModuleStatus.Complete;

        private TrackedRaid? GetRaidForCurrentZone()
        {
            var currentZone = Service.ClientState.TerritoryType;
            var enabledRaids = Settings.TrackedRaids.Where(raid => raid.Tracked.Value);
            var trackedRaidForZone = enabledRaids.FirstOrDefault(raid => raid.Duty.TerritoryType == currentZone);

            return trackedRaidForZone;
        }

        private uint GetFirstMissingRaid()
        {
            var duty = Settings.TrackedRaids
                .Where(raid => raid.Tracked.Value && raid.GetStatus() == ModuleStatus.Incomplete)
                .FirstOrDefault();

            return duty is not null ? duty.Duty.ContentFinderCondition : 1;
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

        public string GetShortTaskLabel() => Strings.Module.Raids.AllianceLabel;

        public string GetLongTaskLabel() => Strings.Module.Raids.AllianceLabel;
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