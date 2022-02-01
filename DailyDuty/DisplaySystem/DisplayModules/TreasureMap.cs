using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class TreasureMap : DisplayModule
    {
        protected Daily.TreasureMapSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings;
        protected override GenericSettings GenericSettings => Settings;

        private readonly HashSet<int> mapLevels;

        private int SelectedMinimumMapLevel
        {
            get
            {
                if (Settings.MinimumMapLevel == 0)
                {
                    Settings.MinimumMapLevel = mapLevels.First();
                }

                return Settings.MinimumMapLevel;
            }
            set => Settings.MinimumMapLevel = value;
        }

        public TreasureMap()
        {
            CategoryString = "Treasure Map";

            mapLevels = DataObjects.MapList.Select(m => m.Level).ToHashSet();
        }

        protected override void DisplayData()
        {
            DisplayLastMapCollectedTime();

            TimeSpanDisplay("Time Until Next Map", TreasureMapModule.TimeUntilNextMap());
        }
        
        protected override void EditModeOptions()
        {
            ImGui.Text("Manually Reset Map Timer");

            if (ImGui.Button($"Reset##{CategoryString}", ImGuiHelpers.ScaledVector2(75, 25)))
            {
                Settings.LastMapGathered = DateTime.Now;
                Service.Configuration.Save();
            }
        }

        protected override void NotificationOptions()
        {
            OnLoginReminderCheckbox(Settings);
            OnTerritoryChangeCheckbox(Settings);

            NotificationField("Map Acquisition Notification", ref Settings.NotifyOnAcquisition, "Confirm Map Acquisition with a chat message.");
            NotificationField("Harvestable Map Notification", ref Settings.HarvestableMapNotification, "Show a notification in chat when there are harvestable Treasure Maps available in the current area.");

            DrawMinimumMapLevelComboBox();
        }
        
        private void DisplayLastMapCollectedTime()
        {
            ImGui.Text("Last Map Collected:");
            ImGui.SameLine();

            ImGui.Text(Settings.LastMapGathered == new DateTime() ? "Never" : $"{Settings.LastMapGathered}");

            ImGui.Spacing();
        }

        private void DrawMinimumMapLevelComboBox()
        {
            if (Settings.HarvestableMapNotification == false) return;

            ImGui.Indent(15 *ImGuiHelpers.GlobalScale);

            ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);

            if (ImGui.BeginCombo("Minimum Map Level", SelectedMinimumMapLevel.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in mapLevels)
                {
                    bool isSelected = element == SelectedMinimumMapLevel;
                    if (ImGui.Selectable(element.ToString(), isSelected))
                    {
                        SelectedMinimumMapLevel = element;
                        Settings.MinimumMapLevel = SelectedMinimumMapLevel;
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

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        public override void Dispose()
        {

        }
    }
}
