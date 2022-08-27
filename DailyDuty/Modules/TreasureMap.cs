using System;
using System.Linq;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;
using DailyDuty.UserInterface.Windows;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace DailyDuty.Modules;

internal class TreasureMap : IModule
{
    public ModuleName Name => ModuleName.TreasureMap;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }
    public ITodoComponent TodoComponent { get; }
    public ITimerComponent TimerComponent { get; }

    private static TreasureMapSettings Settings => Service.ConfigurationManager.CharacterConfiguration.TreasureMap;
    public GenericSettings GenericSettings => Settings;

    public TreasureMap()
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
        private readonly InfoBox nextMap = new();

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


                .EndTable()
                .Draw();

            nextMap
                .AddTitle(Strings.Module.TreasureMap.NextMap)
                .BeginTable()
                .AddRow(
                    Strings.Module.TreasureMap.NextMap,
                    logicModule.GetNextTreasureMap()
                    )

                .EndTable()
                .Draw();
        }
    }

    private unsafe class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }
        public DalamudLinkPayload? DalamudLinkPayload => null;

        private delegate void* TimersWindowDelegate(void* a1, void* a2, byte a3);
        [Signature("E8 ?? ?? ?? ?? 48 8B 4E 10 48 8B 01 44 39 76 20", DetourName = nameof(TimersWindowOpened))]
        private readonly Hook<TimersWindowDelegate>? timersWindowHook = null;

        private delegate long GetNextMapAvailableTimeDelegate(UIState* uiState);
        [Signature("E8 ?? ?? ?? ?? 48 8B F8 E8 ?? ?? ?? ?? 49 8D 9F")]
        private readonly GetNextMapAvailableTimeDelegate getNextMapUnixTimestamp = null!;

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;

            SignatureHelper.Initialise(this);

            timersWindowHook?.Enable();

            Service.Chat.ChatMessage += HandleChat;
        }

        public void Dispose()
        {
            timersWindowHook?.Dispose();

            Service.Chat.ChatMessage -= HandleChat;
        }

        public string GetStatusMessage() => Strings.Module.TreasureMap.MapAvailable;

        public DateTime GetNextReset() => Time.NextDailyReset();

        public void DoReset()
        {
            // Do Nothing
        }

        public ModuleStatus GetModuleStatus() => TimeUntilNextMap() == TimeSpan.Zero ? ModuleStatus.Incomplete : ModuleStatus.Complete;

        private void HandleChat(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (Settings.Enabled.Value == false) return;

            if ((int)type != 2115 || !Service.Condition[ConditionFlag.Gathering])
                return;

            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
                return;

            if (!IsMap(item.ItemId))
                return;

            Settings.LastMapGathered = DateTime.UtcNow;
            Service.ConfigurationManager.Save();
        }

        private bool IsMap(uint itemID)
        {
            var map = GetMapByID(itemID);

            return map != null;
        }

        private TreasureMapItem? GetMapByID(uint itemID)
        {
            return MapList.Maps.FirstOrDefault(map => map.ItemID == itemID);
        }

        private TimeSpan TimeUntilNextMap()
        {
            var lastMapTime = Settings.LastMapGathered;
            var nextAvailableTime = lastMapTime.AddHours(18);

            if (DateTime.UtcNow >= nextAvailableTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return nextAvailableTime - DateTime.UtcNow;
            }
        }

        public string GetNextTreasureMap()
        {
            var span = TimeUntilNextMap();

            if (span == TimeSpan.Zero)
            {
                return Strings.Module.TreasureMap.MapAvailable;
            }

            return TimersOverlayWindow.FormatTimespan(span, TimerStyle.Full);
        }

        private DateTime GetNextMapAvailableTime()
        {
            var unixTimestamp = getNextMapUnixTimestamp(UIState.Instance());

            return unixTimestamp == -1 ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
        }
        
        private void* TimersWindowOpened(void* a1, void* a2, byte a3)
        {
            var result = timersWindowHook!.Original(a1, a2, a3);

            try
            {
                var nextAvailable = GetNextMapAvailableTime();

                if (nextAvailable != DateTime.MinValue)
                {
                    var storedTime = Settings.LastMapGathered;
                    storedTime = storedTime.AddSeconds(-storedTime.Second);

                    var retrievedTime = nextAvailable;
                    retrievedTime = retrievedTime.AddSeconds(-retrievedTime.Second).AddHours(-18);

                    if (storedTime != retrievedTime)
                    {
                        Settings.LastMapGathered = retrievedTime;
                        Service.ConfigurationManager.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Failed to re-sync Treasure Map timer data from Timers Window");
            }

            return result;
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

        public string GetShortTaskLabel() => Strings.Module.TreasureMap.Label;

        public string GetLongTaskLabel() => Strings.Module.TreasureMap.Label;
    }


    private class ModuleTimerComponent : ITimerComponent
    {
        public IModule ParentModule { get; }

        public ModuleTimerComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public TimeSpan GetTimerPeriod() => TimeSpan.FromHours(18);

        public DateTime GetNextReset() => Settings.LastMapGathered + TimeSpan.FromHours(18);
    }
}