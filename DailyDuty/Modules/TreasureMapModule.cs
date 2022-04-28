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
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules
{
    internal class TreasureMapModule :
        IZoneChangeThrottledNotification,
        ILoginNotification,
        ICompletable,
        IChatHandler
    {
        public CompletionType Type => CompletionType.Weekly;
        public GenericSettings GenericSettings => Settings;
        public static TreasureMapSettings Settings => Service.CharacterConfiguration.TreasureMap;
        public string DisplayName => Strings.Module.TreasureMapLabel;
        public Action? ExpandedDisplay => null;

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
    }
}
