using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Addons;
using DailyDuty.Configuration.Components;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.Utilities;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
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
            .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse == 17)
            .OrderByDescending(cfc => cfc.SortKey)
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

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            InfoBox.DrawGenericSettings(this);

            if (Settings.TrackedRaids is { } trackedRaids)
            {
                InfoBox.Instance
                    .AddTitle(Strings.Module.Raids.TrackedNormalRaids)
                    .BeginTable(0.70f)
                    .AddRows(trackedRaids.OfType<IInfoBoxTableConfigurationRow>())
                    .EndTable()
                    .Draw();
            }

            InfoBox.Instance
                .AddTitle(Strings.Module.Raids.Regenerate)
                .AddString(Strings.Module.Raids.RegenerateHelp, Colors.Orange)
                .AddAction(RegenerateRaidList)
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Module.Raids.ClickableLinkLabel)
                .AddString(Strings.Module.Raids.ClickableLink)
                .AddConfigCheckbox(Strings.Common.Enabled, Settings.EnableClickableLink)
                .Draw();

            InfoBox.DrawNotificationOptions(this);
        }

        private void RegenerateRaidList()
        {
            var keys = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;

            ImGuiHelpers.ScaledDummy(15.0f);

            ImGui.BeginDisabled(!keys);
            if (ImGui.Button(Strings.Module.Raids.Regenerate, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale)))
            {
                RegenerateTrackedRaids();
            }
            ImGui.EndDisabled();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ISelectable Selectable => new StatusSelectable(ParentModule, this, ParentModule.LogicComponent.GetModuleStatus);

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }
        
        public void Draw()
        {
            InfoBox.DrawGenericStatus(this);

            if (Settings.TrackedRaids.Any(raid => raid.Tracked.Value))
            {
                InfoBox.Instance
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
                InfoBox.Instance
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

            DalamudLinkPayload = Service.PayloadManager.AddChatLink(ChatPayloads.NormalRaidsDutyFinder, OpenDutyFinder);

            Service.AddonManager.Get<DutyFinderAddon>().Refresh += OnSelectionChanged;
            Service.Chat.ChatMessage += OnChatMessage;
        }

        public void Dispose()
        {
            Service.AddonManager.Get<DutyFinderAddon>().Refresh -= OnSelectionChanged;
            Service.Chat.ChatMessage -= OnChatMessage;
        }

        private void OnSelectionChanged(object? sender, IntPtr e)
        {
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

        public string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.Module.Raids.RaidsRemaining}";

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

        public string GetShortTaskLabel() => Strings.Module.Raids.NormalLabel;

        public string GetLongTaskLabel() => Strings.Module.Raids.NormalLabel;
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