using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.System.Modules
{
    internal class TreasureMapModule : Module
    {
        protected readonly Configuration.DailyTreasureMapSettings Settings = Service.Configuration.TreasureMapSettings;

        public TreasureMapModule()
        {
            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
            Service.Chat.ChatMessage += OnChatMap;
        }

        public override void Update()
        {

        }
        
        protected void OnTerritoryChanged(object? sender, ushort e)
        {
            if (Service.Condition[ConditionFlag.BoundByDuty] == true) return;
            if (Settings.Enabled == false) return;

            if (TimeUntilNextMap() == TimeSpan.Zero)
            {
                Util.PrintMessage("You have a Treasure Map Allowance Available.");
            }

            var maps = GetMapsForTerritory(e);

            if (Settings.NotificationEnabled && TimeUntilNextMap() == TimeSpan.Zero)
            {
                foreach (var map in maps)
                {
                    if (map.Level >= Settings.MinimumMapLevel)
                    {
                        var mapName = Service.DataManager.GetExcelSheet<Item>()!.GetRow(map.ItemID)!.Name;

                        Util.PrintMessage($"A '{mapName}' is available for harvest in this area. Via: {GetHarvestingTypeByTerritory(map.HarvestData, e)}");
                    }
                }
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

        // Based on https://github.com/Ottermandias/Accountant/blob/main/Accountant/Manager/TimerManager.MapManager.cs#L75
        protected void OnChatMap(XivChatType type, uint senderid, ref SeString sender, ref SeString message, ref bool ishandled)
        {
            if (Service.Configuration.TreasureMapSettings.Enabled == false) return;

            if ((int)type != 2115 || !Service.Condition[ConditionFlag.Gathering])
                return;

            if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
                return;

            if (!IsMap(item.ItemId))
                return;

            Service.Configuration.TreasureMapSettings.LastMapGathered = DateTime.Now;
            Service.Configuration.Save();
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

        public static TimeSpan TimeUntilNextMap()
        {
            var lastMapTime = Service.Configuration.TreasureMapSettings.LastMapGathered;
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



        public override void Dispose()
        {
            Service.ClientState.TerritoryChanged -= OnTerritoryChanged;
            Service.Chat.ChatMessage -= OnChatMap;
        }
    }
}
