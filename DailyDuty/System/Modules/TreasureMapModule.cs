using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.GeneratedSheets;
using Util = DailyDuty.System.Utilities.Util;

namespace DailyDuty.System.Modules
{
    internal class TreasureMapModule : Module
    {
        protected Daily.TreasureMapSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings;

        public TreasureMapModule()
        {
            Service.Chat.ChatMessage += OnChatMap;
        }
        
        protected override void OnTerritoryChanged(object? sender, ushort e)
        {
            if (ConditionManager.IsBoundByDuty() == true) return;

            if (Settings.Enabled && Settings.TerritoryChangeReminder)
            {
                if (TimeUntilNextMap() == TimeSpan.Zero)
                {
                    Util.PrintTreasureMap("You have a Treasure Map Allowance Available.");
                }
            }

            if (Settings.HarvestableMapNotification == true && TimeUntilNextMap() == TimeSpan.Zero)
            {
                var maps = GetMapsForTerritory(e);

                foreach (var map in maps)
                {
                    if (map.Level >= Settings.MinimumMapLevel)
                    {
                        var mapName = Service.DataManager.GetExcelSheet<Item>()!.GetRow(map.ItemID)!.Name; 

                        Util.PrintTreasureMap($"A '{mapName}' is available for harvest in this area.");
                    }
                }
            }
        }
        
        // Based on https://github.com/Ottermandias/Accountant/blob/main/Accountant/Manager/TimerManager.MapManager.cs#L75
        protected void OnChatMap(XivChatType type, uint senderid, ref SeString sender, ref SeString message, ref bool ishandled)
        {
            if (Settings.Enabled == false) return;

            if ((int)type != 2115 || !Service.Condition[ConditionFlag.Gathering])
                return;

            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
                return;

            if (!IsMap(item.ItemId))
                return;

            if (Settings.NotifyOnAcquisition == true)
            {
                var mapName = item.Item!.Name.ToString();

                Util.PrintTreasureMap($"A '{mapName}' has been gathered.");
                Util.PrintTreasureMap($"Your next map will be available on {DateTime.Now.AddHours(18)}");
            }

            Settings.LastMapGathered = DateTime.Now;
            Service.Configuration.Save();
        }

        protected override void OnLoginDelayed()
        {
            if (Settings.Enabled && Settings.LoginReminder)
            {
                if (IsTreasureMapAvailable())
                {
                    Util.PrintTreasureMap("You have a Treasure Map Allowance Available.");
                }
            }
        }

        public static bool IsTreasureMapAvailable()
        {
            return TimeUntilNextMap() == TimeSpan.Zero;
        }

        public static TimeSpan TimeUntilNextMap()
        {
            var lastMapTime = Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings.LastMapGathered;
            var nextAvailableTime = lastMapTime.AddHours(18);

            if (DateTime.Now >= nextAvailableTime)
            {
                return TimeSpan.Zero;
            }
            else
            {
                return nextAvailableTime - DateTime.Now;
            }
        }

        protected string GetHarvestingTypeByTerritory(Dictionary<DataObjects.HarvestType, List<uint>> mapHarvestData, uint territory)
        {
            var harvestMethods = mapHarvestData
                .Where(data => data.Value.Contains(territory))
                .Select(e => e.Key.ToString());

            string methods = "{ " + string.Join(", ", harvestMethods) + " }";

            return methods;
        }

        protected HashSet<DataObjects.TreasureMap> GetMapsForTerritory(uint territory)
        {
            return DataObjects.MapList
                .Where(m => m.HarvestData.Any(data => data.Value.Contains(territory)))
                .Where(m => m.Level >= Settings.MinimumMapLevel)
                .ToHashSet();
        }

        protected DataObjects.TreasureMap? GetMapByID(uint itemID)
        {
            foreach (var map in DataObjects.MapList)
            {
                if (map.ItemID == itemID)
                {
                    return map;
                }
            }

            return null;
        }

        protected bool IsMap(uint itemID)
        {
            var map = GetMapByID(itemID);

            return map != null;
        }

        public override void Dispose()
        {
            base.Dispose();

            Service.Chat.ChatMessage -= OnChatMap;
        }

        public override bool IsCompleted()
        {
            return IsTreasureMapAvailable() == false;
        }

        public override void DoDailyReset(Configuration.CharacterSettings settings)
        {
            // Treasure Maps don't reset daily
        }

        public override void DoWeeklyReset(Configuration.CharacterSettings settings)
        {
            // Treasure Maps don't reset weekly
        }
    }
}
