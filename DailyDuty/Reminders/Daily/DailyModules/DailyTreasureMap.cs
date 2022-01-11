
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Reminders.Daily.DailyModules
{
    internal class DailyTreasureMap : ReminderModule
    {
        private readonly Configuration.DailyTreasureMapSettings settings = Service.Configuration.TreasureMapSettings;
        private readonly HashSet<int> mapLevels;
        private int selectedMinimumMapLevel = 0;

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

            mapLevels = mapList.Select(m => m.Level).ToHashSet();

            selectedMinimumMapLevel = settings.MinimumMapLevel;

            OnTerritoryChanged(this, thisTerritory);
        }

        private void OnTerritoryChanged(object? sender, ushort e)
        {
            if (settings.Enabled == false) return;

            if (TimeUntilNextMap() == TimeSpan.Zero)
            {
                Util.PrintMessage("You have a Treasure Map Allowance Available.");
            }

            var maps = GetMapsForTerritory(e);

            if (settings.NotificationEnabled && TimeUntilNextMap() == TimeSpan.Zero)
            {
                foreach (var map in maps)
                {
                    if (map.Level >= settings.MinimumMapLevel)
                    {
                        var mapName = Service.DataManager.GetExcelSheet<Item>()!.GetRow(map.ItemID)!.Name;

                        Util.PrintMessage($"A '{mapName}' is available for harvest in this area. Via: {GetHarvestingTypeByTerritory(map.HarvestData, e)}");
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

        // Based on https://github.com/Ottermandias/Accountant/blob/main/Accountant/Manager/TimerManager.MapManager.cs#L75
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
            if (ImGui.Checkbox("Enabled", ref settings.Enabled))
            {
                PluginLog.Information($"Treasure Map Module {(settings.Enabled ? "Enabled" : "Disabled")}");
            }

            if (settings.Enabled)
            {
                if (ImGui.Checkbox("Notifications", ref settings.NotificationEnabled))
                {
                    PluginLog.Information($"Treasure Map Module Notifications {(settings.NotificationEnabled ? "Enabled" : "Disabled")}");
                }

                DrawTimeStatusDisplayAndCountdown();

                DrawMinimumMapLevelComboBox();
            }

            ImGui.Spacing();
            ImGui.Separator();
        }

        private void DrawTimeStatusDisplayAndCountdown()
        {
            if (Service.Configuration.TreasureMapSettings.LastMapGathered == new DateTime())
            {
                ImGui.Text($"Last Map Collected: Never");

            }
            else
            {
                ImGui.Text($"Last Map Collected: {Service.Configuration.TreasureMapSettings.LastMapGathered}");
            }

            var timeSpan = TimeUntilNextMap();
            ImGui.Text($"Time Until Next Map: ");
            ImGui.SameLine();

            if (timeSpan == TimeSpan.Zero)
            {
                ImGui.TextColored(new(0, 255, 0, 255), $" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
            else
            {
                ImGui.Text($" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
        }

        private void DrawMinimumMapLevelComboBox()
        {
            ImGui.PushItemWidth(50);

            if (ImGui.BeginCombo("Minimum Map Level", selectedMinimumMapLevel.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in mapLevels)
                {
                    bool isSelected = element == selectedMinimumMapLevel;
                    if (ImGui.Selectable(element.ToString(), isSelected))
                    {
                        selectedMinimumMapLevel = element;
                        settings.MinimumMapLevel = selectedMinimumMapLevel;
                        OnTerritoryChanged(this, Service.ClientState.TerritoryType);
                        Service.Configuration.Save();
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            ImGuiComponents.HelpMarker("Only show notifications that a map is available if the map is at least this level.");

            ImGui.PopItemWidth();
        }

        private HashSet<TreasureMap> GetMapsForTerritory(uint territory)
        {
            return mapList
                .Where(m => m.HarvestData.Any(data => data.Value.Contains(territory)))
                .Where(m => m.Level >= settings.MinimumMapLevel)
                .ToHashSet();
        }

        private TreasureMap? GetMapByID(uint itemID)
        {
            foreach (var map in mapList)
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

        private readonly List<TreasureMap> mapList = new()
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
            },

            // timeworn peisteskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){137,397,134,152,141}},
                    {HarvestType.Harvesting, new(){146}},
                    {HarvestType.Mining, new(){147, 397}},
                    {HarvestType.Quarrying, new() {138,152,135,140}}
                },
                ItemID = 6692,
                Level = 50
            },

            // timeworn archaeoskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){398}},
                    {HarvestType.Harvesting, new(){398,400,397}},
                    {HarvestType.Mining, new(){398,397}},
                    {HarvestType.Quarrying, new(){397}}
                },
                ItemID = 12241,
                Level = 55
            },

            // timeworn wyvernskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){400, 401}},
                    {HarvestType.Harvesting, new(){398, 400, 399, 401, 397}},
                    {HarvestType.Mining, new(){398, 397, 399, 401}},
                    {HarvestType.Quarrying, new(){400}}
                },
                ItemID = 12242,
                Level = 60
            },

            // timeworn dragonskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){400, 401}},
                    {HarvestType.Harvesting, new(){398, 400, 399, 401, 397}},
                    {HarvestType.Mining, new(){398, 397, 399, 401}},
                    {HarvestType.Quarrying, new(){400}}
                },
                ItemID = 12243,
                Level = 60
            },

            // timeworn gaganaskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){621, 612}},
                    {HarvestType.Harvesting, new(){614,610,622,613}},
                    {HarvestType.Mining, new(){621, 614}},
                    {HarvestType.Quarrying, new(){622}}
                },
                ItemID = 17835,
                Level = 70
            },

            // timeworn gazelleskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){621, 612}},
                    {HarvestType.Harvesting, new(){614, 610, 622, 613}},
                    {HarvestType.Mining, new(){621, 614}},
                    {HarvestType.Quarrying, new(){622}}
                },
                ItemID = 17836,
                Level = 70
            },

            // timeworn gliderskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){813,815}},
                    {HarvestType.Harvesting, new(){814, 817, 813}},
                    {HarvestType.Mining, new(){813,817}},
                    {HarvestType.Quarrying, new(){814,815}}
                },
                ItemID = 26744,
                Level = 80
            },

            // timeworn zonureskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){813,815}},
                    {HarvestType.Harvesting, new(){814,817,813}},
                    {HarvestType.Mining, new(){813,817}},
                    {HarvestType.Quarrying, new(){814,815}}
                },
                ItemID = 26745,
                Level = 80
            },

            // timeworn saigaskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    {HarvestType.Logging, new(){960, 961}},
                    {HarvestType.Harvesting, new(){960, 961}},
                    {HarvestType.Mining, new(){960, 961}},
                    {HarvestType.Quarrying, new(){960, 961}},
                },
                ItemID = 36611,
                Level = 90
            },

            // timeworn kumbhiraskin map
            new TreasureMap()
            {
                HarvestData = new()
                {
                    { HarvestType.Logging, new() { 960, 961 } },
                    { HarvestType.Harvesting, new() { 960, 961 } },
                    { HarvestType.Mining, new() { 960, 961 } },
                    { HarvestType.Quarrying, new() { 960, 961 } },
                },
                ItemID = 36612,
                Level = 90
            }
        };
    }
}
