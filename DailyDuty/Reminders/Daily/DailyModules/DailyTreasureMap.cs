
using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Reminders.Daily.DailyModules
{
    internal class DailyTreasureMap : ReminderModule
    {
        private Configuration.DailyTreasureMapSettings Settings = Service.Configuration.TreasureMapSettings;

        public enum HarvestType
        {
            Logging,
            Harvesting,
            Mining,
            Quarrying
        }

        private class TreasureMap
        {
            public Dictionary<HarvestType, List<uint>> HarvestData { get; init; } = new();
            public uint ItemID { get; init; }
            public int Level { get; init; }
        }

        public DailyTreasureMap()
        {
            CategoryString = "Daily Treasure Map";

            Service.ClientState.TerritoryChanged += OnTerritoryChanged;
            Service.Chat.ChatMessage += OnChatMap;

            var thisTerritory = Service.ClientState.TerritoryType;

            OnTerritoryChanged(this, thisTerritory);
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            var maps = GetMapsForTerritory(e);

            if (maps.Count == 0)
            {
                PluginLog.Information($"No Maps found for territory {e}");
                return;
            }

            if (Settings.NotificationEnabled)
            {
                foreach (var map in maps)
                {
                    if (map.Level >= Settings.MinimumMapLevel)
                    {
                        var mapName = Service.DataManager.GetExcelSheet<Item>()!.GetRow(map.ItemID)!.Name;

                        Service.Chat.Print($"A '{mapName}' is available for harvest in this area. Via: {GetHarvestingTypeByTerritory(map.HarvestData, e)}");
                    }
                }
            }
        }

        private string GetHarvestingTypeByTerritory(Dictionary<HarvestType, List<uint>> mapHarvestData, uint territory)
        {
            var harvestMethods = mapHarvestData
                .Where(data => data.Value.Contains(territory))
                .Select(e => e.Key.ToString());

            string methods = "{ " + string.Join(", ", harvestMethods) + " }";

            return methods;
        }

        private void OnChatMap(XivChatType type, uint senderid, ref SeString sender, ref SeString message, ref bool ishandled)
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

        protected override void DrawContents()
        {
            if (ImGui.Checkbox("Enabled", ref Settings.Enabled))
            {
                PluginLog.Information($"Treasure Map Module {(Settings.Enabled ? "Enabled" : "Disabled")}");
            }

            if (Settings.Enabled)
            {
                if (ImGui.Checkbox("Notifications", ref Settings.NotificationEnabled))
                {
                    PluginLog.Information($"Treasure Map Module Notifications {(Settings.Enabled ? "Enabled" : "Disabled")}");
                }

                ImGui.Text($"Last Map Collected: {Service.Configuration.TreasureMapSettings.LastMapGathered}");

                var timeSpan = TimeUntilNextMap();
                ImGui.Text($"Time Until Next Map: {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
        }

        private HashSet<TreasureMap> GetMapsForTerritory(uint territory)
        {
            HashSet<TreasureMap> results = new();

            foreach (var dataSet in MapList)
            {
                foreach (var (harvest, territories) in dataSet.HarvestData)
                {
                    foreach (var terr in territories)
                    {
                        if (terr == territory)
                        {
                            results.Add(dataSet);
                        }
                    }
                }
            }

            return results;
        }

        private TreasureMap? GetMapByID(uint itemID)
        {
            foreach (var map in MapList)
            {
                if (map.ItemID == itemID)
                {
                    return map;
                }
            }

            return null;
        }

        private bool IsMap(uint itemID)
        {
            var map = GetMapByID(itemID);

            return map != null;
        }

        private TimeSpan TimeUntilNextMap()
        {
            var lastMapTime = Service.Configuration.TreasureMapSettings.LastMapGathered;
            var nextAvailableTime = lastMapTime + TimeSpan.FromHours(18);

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

        private List<TreasureMap> MapList = new()
        {
            // timeworn leather map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new() {137}},
                    {HarvestType.Harvesting, new() {153, 137}},
                    {HarvestType.Mining, new() {153, 155}},
                    {HarvestType.Quarrying, new() {147}}
                },
                ItemID = 6688,
                Level = 40,
            },

            // timeworn goatskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new() {155}},
                    {HarvestType.Harvesting, new() {139}},
                    {HarvestType.Mining, new() {139, 145}},
                    {HarvestType.Quarrying, new() {139}}
                },
                ItemID = 6689,
                Level = 45
            },

            //timeworn toadskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new() {137, 397, 134, 152, 141}},
                    {HarvestType.Harvesting, new() {146}},
                    {HarvestType.Mining, new() {147,397}},
                    {HarvestType.Quarrying, new(){138,152,135,140}}
                },
                ItemID = 6690,
                Level = 50
            },

            // timeworn boarskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){137, 397, 134,152,141}},
                    {HarvestType.Harvesting, new(){146}},
                    {HarvestType.Mining, new(){147, 397}},
                    {HarvestType.Quarrying, new(){138, 152, 135, 140}}
                },
                ItemID = 6691,
                Level = 50
            }

        };
    }
}
