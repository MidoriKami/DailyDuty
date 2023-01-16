using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.Configuration;
using DailyDuty.DataModels;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.Utilities;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Configuration;
using KamiLib.Drawing;
using KamiLib.Interfaces;
using KamiLib.Misc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Modules;

public class RaidsNormalSettings : GenericSettings
{
    public List<TrackedRaid> TrackedRaids = new();
    public Setting<bool> EnableClickableLink = new(true);
}

internal class RaidsNormal : IModule
{
    public ModuleName Name => ModuleName.NormalRaids;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static RaidsNormalSettings Settings => Service.ConfigurationManager.CharacterConfiguration.RaidsNormal;
    public GenericSettings GenericSettings => Settings;

    public RaidsNormal()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
        TodoComponent = new ModuleTodoComponent(this);
        TimerComponent = new ModuleTimerComponent(this);
        
        Service.ConfigurationManager.OnCharacterDataLoaded += ConfigurationLoaded;
    }

    private void ConfigurationLoaded(object? sender, CharacterConfiguration e)
    {
        if (!Settings.TrackedRaids.Any() || IsDataStale())
        {
            PluginLog.Information("New Limited Normal Raid Found. Reloading duty information.");
            
            Settings.TrackedRaids.Clear();

            foreach (var limitedDuty in DutyLists.Instance.LimitedSavage)
            {
                var cfc = LuminaCache<ContentFinderCondition>.Instance.First(entry => entry.TerritoryType.Row == limitedDuty);
                
                Settings.TrackedRaids.Add(new TrackedRaid(cfc));
            }
            
            Service.ConfigurationManager.Save();
        }
    }

    private static bool IsDataStale() => Settings.TrackedRaids.Any(trackedTask => !DutyLists.Instance.LimitedSavage.Contains(trackedTask.Duty.TerritoryType));
    
    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataLoaded -= ConfigurationLoaded;
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

            if (Settings.TrackedRaids is { } trackedRaids)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Raids_TrackedRaids)
                    .BeginTable(0.70f)
                    .AddConfigurationRows(trackedRaids)
                    .EndTable()
                    .Draw();
            }

            InfoBox.Instance
                .AddTitle(Strings.Common_ClickableLink)
                .AddString(Strings.DutyFinder_ClickableLink)
                .AddConfigCheckbox(Strings.Common_Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.Instance.DrawNotificationOptions(this);
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.Status);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }
        
        public void Draw()
        {
            InfoBox.Instance.DrawGenericStatus(this);

            if (Settings.TrackedRaids.Any(raid => raid.Tracked))
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status_ModuleData)
                    .BeginTable(0.70f)
                    .AddDataRows(Settings.TrackedRaids.Where(raid => raid.Tracked))
                    .EndTable()
                    .Draw();
            }
            else
            {
                InfoBox.Instance
                    .AddTitle(Strings.Status_ModuleData, out var innerWidth)
                    .AddStringCentered(Strings.Raids_NothingTracked, innerWidth, Colors.Orange)
                    .Draw();
            }
            
            InfoBox.Instance.DrawSuppressionOption(this);
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload { get; }
        public bool LinkPayloadActive => Settings.EnableClickableLink;

        private readonly AgentContentsFinder* contentsFinderAgentInterface = AgentContentsFinder.Instance();

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            DalamudLinkPayload = ChatPayloadManager.Instance.AddChatLink(ChatPayloads.NormalRaidsDutyFinder, OpenDutyFinder);

            DutyFinderAddon.Instance.Refresh += OnSelectionChanged;
            Service.Chat.ChatMessage += OnChatMessage;
        }

        public void Dispose()
        {
            DutyFinderAddon.Instance.Refresh -= OnSelectionChanged;
            Service.Chat.ChatMessage -= OnChatMessage;
        }

        private void OnSelectionChanged(object? sender, nint e)
        {
            var enabledRaids = Settings.TrackedRaids.Where(raid => raid.Tracked).ToList();
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
            if (!Settings.Enabled) return;

            // If message is a loot message
            if (((int)type & 0x7F) != 0x3E) return;

            // If we are in a zone that we are tracking
            if (GetRaidForCurrentZone() is not { } trackedRaid) return;

            // If the message does NOT contain a player payload
            if (message.Payloads.FirstOrDefault(p => p is PlayerPayload) is PlayerPayload) return;

            // If the message DOES contain an item
            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload { Item: { } item } ) return;

            switch (item.ItemUICategory.Row)
            {
                case 61 when item.ItemAction.Row == 0:
                    trackedRaid.CurrentDropCount += 1;
                    Service.ConfigurationManager.Save();
                    break;
            }
        }

        
        private void OpenDutyFinder(uint arg1, SeString arg2)
        {
            AgentContentsFinder.Instance()->OpenRegularDuty(GetFirstMissingRaid());
        }

        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Raids_RaidsRemaining}";

        public DateTime GetNextReset() => Time.NextWeeklyReset();

        public void DoReset()
        {
            foreach (var raid in Settings.TrackedRaids)
            {
                raid.CurrentDropCount = 0;
            }
        }

        private static int GetIncompleteCount() => Settings.TrackedRaids.Count(raid => raid.Tracked && raid.GetStatus() == ModuleStatus.Incomplete);
        public ModuleStatus GetModuleStatus() => GetIncompleteCount() > 0 ? ModuleStatus.Incomplete : ModuleStatus.Complete;

        private static TrackedRaid? GetRaidForCurrentZone()
        {
            var currentZone = Service.ClientState.TerritoryType;
            var enabledRaids = Settings.TrackedRaids.Where(raid => raid.Tracked);
            var trackedRaidForZone = enabledRaids.FirstOrDefault(raid => raid.Duty.TerritoryType == currentZone);

            return trackedRaidForZone;
        }

        private static uint GetFirstMissingRaid()
        {
            var duty = Settings.TrackedRaids
                .Where(raid => raid.Tracked && raid.GetStatus() == ModuleStatus.Incomplete)
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

        public string GetShortTaskLabel() => Strings.Raids_NormalLabel;

        public string GetLongTaskLabel() => Strings.Raids_NormalLabel;
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