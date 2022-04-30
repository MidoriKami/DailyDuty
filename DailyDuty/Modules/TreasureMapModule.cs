using System;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Action = System.Action;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules
{
    internal unsafe class TreasureMapModule :
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IChatHandler,
        IDisposable
    {
        public CompletionType Type => CompletionType.Daily;
        public GenericSettings GenericSettings => Settings;
        public static TreasureMapSettings Settings => Service.CharacterConfiguration.TreasureMap;
        public string DisplayName => Strings.Module.TreasureMapLabel;
        public Action? ExpandedDisplay => null;


        private delegate void* TimersWindowDelegate(void* a1, void* a2, byte a3);
        [Signature("E8 ?? ?? ?? ?? 48 8B 4E 10 48 8B 01 44 39 76 20", DetourName = nameof(TimersWindowOpened))]
        private readonly Hook<TimersWindowDelegate>? timersWindowHook = null!;

        private delegate long GetNextMapAvailableTimeDelegate(UIState* uiState);
        [Signature("E8 ?? ?? ?? ?? 48 8B F8 E8 ?? ?? ?? ?? 49 8D 9F")]
        private readonly GetNextMapAvailableTimeDelegate GetNextMapUnixTimestamp = null!;

        private void* TimersWindowOpened(void* a1, void* a2, byte a3)
        {
            var result = timersWindowHook!.Original(a1, a2, a3);

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
                    Service.LogManager.LogMessage(ModuleType.TreasureMap, $"ReSyncing Time - LastGathered: {retrievedTime.ToLocalTime()}");

                    Service.CharacterConfiguration.Save();
                }
            }

            return result;
        }

        public TreasureMapModule()
        {
            SignatureHelper.Initialise(this);

            timersWindowHook?.Enable();
        }

        public void Dispose()
        {
            timersWindowHook?.Dispose();
        }

        public void SendNotification()
        {
            if (!IsCompleted() && !Condition.IsBoundByDuty())
            {
                Chat.Print(Strings.Module.TreasureMapLabel, Strings.Module.TreasureMapAvailableMessage);
            }
        }

        public bool IsCompleted() => TimeUntilNextMap() != TimeSpan.Zero;

        public void HandleChat(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (Settings.Enabled == false) return;

            if ((int)type != 2115 || !Service.Condition[ConditionFlag.Gathering])
                return;

            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
                return;

            if (!IsMap(item.ItemId))
                return;

            Service.LogManager.LogMessage(ModuleType.TreasureMap, "Treasure Map Collected");

            Settings.LastMapGathered = DateTime.UtcNow;
            Service.CharacterConfiguration.Save();
        }

        private bool IsMap(uint itemID)
        {
            var map = GetMapByID(itemID);

            return map != null;
        }

        private TreasureMap? GetMapByID(uint itemID)
        {
            return MapList.Maps.FirstOrDefault(map => map.ItemID == itemID);
        }

        public TimeSpan TimeUntilNextMap()
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

        private DateTime GetNextMapAvailableTime()
        {
            var unixTimestamp = GetNextMapUnixTimestamp(UIState.Instance());

            return unixTimestamp == -1 ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
        }
    }
}
