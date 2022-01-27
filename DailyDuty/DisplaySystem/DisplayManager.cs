using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.System;
using DailyDuty.System.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal class DisplayManager : Window
    {
        private Tab currentTab = Tab.ToDo;
        private readonly Vector2 windowSize = new(450, 500);

        private readonly Dictionary<Tab, TabCategory> settingsCategories = new();

        public enum Tab
        {
            ToDo,
            Daily,
            Weekly,
            Settings
        }

        public DisplayManager(ModuleManager moduleManager) : base("Daily Duty")
        {
            settingsCategories.Add(Tab.ToDo, new ToDoTab(moduleManager));
            settingsCategories.Add(Tab.Daily, new DailyTab());
            settingsCategories.Add(Tab.Weekly, new WeeklyTab());
            settingsCategories.Add(Tab.Settings, new SettingsTab());

            IsOpen = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(windowSize.X, windowSize.Y),
                MaximumSize = new(windowSize.X, windowSize.Y)
            };

            Flags |= ImGuiWindowFlags.NoResize;
            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(10, 5));

            DrawDailyCountdown();
            ImGui.SameLine(ImGui.GetWindowWidth() - 205 * ImGuiHelpers.GlobalScale);
            DrawWeeklyCountdown();

            ImGui.Spacing();

            DrawTabs();

            settingsCategories[currentTab].Draw();

            DrawSaveAndCloseButtons();

            ImGui.PopStyleVar();
        }

        // Progress bars based on https://github.com/Fr4nsson
        private void DrawDailyCountdown()
        {
            var now = DateTime.UtcNow;
            var totalHours = Util.NextDailyReset() - now;
            var percentage = (float) (1 - totalHours / TimeSpan.FromDays(1) );

            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 255));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(37 /255.0f, 168 / 255.0f, 1.0f, 0.5f));
            ImGui.ProgressBar(percentage, ImGuiHelpers.ScaledVector2(200, 20), $"Daily Reset: {totalHours.Hours:00}:{totalHours.Minutes:00}:{totalHours.Seconds:00}");
            ImGui.PopStyleColor(2);
        }

        private void DrawWeeklyCountdown()
        {
            var now = DateTime.UtcNow;
            var delta = Util.NextWeeklyReset() - now;

            var percentage = (float)(1 - delta / TimeSpan.FromDays(7));

            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 255));
            ImGui.PushStyleColor(ImGuiCol.PlotHistogram, new Vector4(176 / 255.0f, 38 / 255.0f, 236 / 255.0f, 0.5f));

            string daysDisplay = ""; 

            if (delta.Days == 1)
            {
                daysDisplay = $"{delta.Days} day, ";
            }
            else if (delta.Days > 1)
            {
                daysDisplay = $"{delta.Days} days, ";
            }

            ImGui.ProgressBar(percentage, ImGuiHelpers.ScaledVector2(200, 20), $"Weekly Reset: {daysDisplay}{delta.Hours:00}:{delta.Minutes:00}:{delta.Seconds:00}");
            ImGui.PopStyleColor(2);
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("Daily Duty Settings", ImGuiTabBarFlags.NoTooltip))
            {
                foreach (var (tab, data) in settingsCategories)
                {
                    if (ImGui.BeginTabItem(data.TabName))
                    {
                        currentTab = tab;
                        ImGui.EndTabItem();
                    }
                }
            }
        }

        public override void OnClose()
        {
            SettingsTab.EditModeEnabled = false;
            base.OnClose();
        }

        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            ImGui.SetCursorPos(new Vector2(5, ImGui.GetWindowHeight() - 30 * ImGuiHelpers.GlobalScale));

            if (ImGui.Button($"Save", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 155 * ImGuiHelpers.GlobalScale);

            if (ImGui.Button($"Save & Close", ImGuiHelpers.ScaledVector2(150, 25)))
            {
                Service.Configuration.Save();
                IsOpen = false;
            }

            ImGui.Spacing();
        }

        public void Dispose()
        {
            foreach (var tab in settingsCategories)
            {
                tab.Value.Dispose();
            }
        }
    }
}
