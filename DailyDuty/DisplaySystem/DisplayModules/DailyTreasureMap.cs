using System;
using System.Collections.Generic;
using System.Linq;
using CheapLoc;
using DailyDuty.ConfigurationSystem;
using DailyDuty.Data;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayModules
{
    internal class DailyTreasureMap : DisplayModule
    {
        protected Daily.TreasureMapSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings;
        protected override GenericSettings GenericSettings => Settings;

        private readonly HashSet<int> mapLevels;

        private int SelectedMinimumMapLevel
        {
            get => Settings.MinimumMapLevel;
            set => Settings.MinimumMapLevel = value;
        }

        public DailyTreasureMap()
        {
            CategoryString = Loc.Localize("DTM", "Daily Treasure Map");

            mapLevels = DataObjects.MapList.Select(m => m.Level).ToHashSet();
        }

        protected override void DisplayData()
        {
            DisplayLastMapCollectedTime();
            TimeUntilNextMap();
        }

        protected override void DisplayOptions()
        {
        }

        protected override void EditModeOptions()
        {
            var stringReset = Loc.Localize("Reset", "Reset");

            ImGui.Text(Loc.Localize("DTM_Reset", "Manually Reset Map Timer"));

            if (ImGui.Button($"{stringReset}##{CategoryString}", ImGuiHelpers.ScaledVector2(75, 25)))
            {
                Settings.LastMapGathered = DateTime.Now;
                Service.Configuration.Save();
            }
        }

        protected override void NotificationOptions()
        {
            DrawMinimumMapLevelComboBox();
        }

        private static void TimeUntilNextMap()
        {
            var timeSpan = TreasureMapModule.TimeUntilNextMap();
            ImGui.Text(Loc.Localize("DTM_TimeUntil", "Time Until Next Map: "));
            ImGui.SameLine();

            if (timeSpan == TimeSpan.Zero)
            {
                ImGui.TextColored(new(0, 255, 0, 255), $" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }
            else
            {
                ImGui.Text($" {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
            }

            ImGui.Spacing();
        }

        private void DisplayLastMapCollectedTime()
        {
            if (Settings.LastMapGathered == new DateTime())
            {
                ImGui.Text(Loc.Localize("DTM_LastNever", "Last Map Collected: Never"));
            }
            else
            {
                ImGui.Text(Loc.Localize("DTM_Last", "Last Map Collected: {0}").Format(Service.Configuration
                    .CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings.LastMapGathered));
            }

            ImGui.Spacing();
        }

        private void DrawMinimumMapLevelComboBox()
        {
            var stringMinimumLevel = Loc.Localize("DTM_MinLevel", "Minimum Map Level");

            ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);

            if (ImGui.BeginCombo(stringMinimumLevel, SelectedMinimumMapLevel.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var element in mapLevels)
                {
                    bool isSelected = element == SelectedMinimumMapLevel;
                    if (ImGui.Selectable(element.ToString(), isSelected))
                    {
                        SelectedMinimumMapLevel = element;
                        Settings.MinimumMapLevel = SelectedMinimumMapLevel;
                        Service.Configuration.Save();
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            var locString = Loc.Localize("DTM_HelpMinMapLevel", "Only show notifications that a map is available if the map is at least this level.");
            ImGuiComponents.HelpMarker(locString);

            ImGui.PopItemWidth();
            ImGui.Spacing();
        }

        public override void Dispose()
        {

        }
    }
}
