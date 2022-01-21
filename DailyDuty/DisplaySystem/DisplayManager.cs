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
            Weekly
        }

        public DisplayManager(ModuleManager moduleManager) : base("Daily Duty")
        {
            settingsCategories.Add(Tab.ToDo, new ToDoTab(moduleManager));
            settingsCategories.Add(Tab.Daily, new DailyTab());
            settingsCategories.Add(Tab.Weekly, new WeeklyTab());

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
            ImGui.SameLine();
            DrawWeeklyCountdown();

            DrawTabs();

            settingsCategories[currentTab].Draw();

            DrawSaveAndCloseButtons();

            ImGui.PopStyleVar();
        }

        private void DrawDailyCountdown()
        {
            var now = DateTime.UtcNow;
            var totalHours = Util.NextDailyReset() - now;

            ImGui.Text($"Time until daily reset: {totalHours.Hours:00}:{totalHours.Minutes:00}:{totalHours.Seconds:00}");
        }

        private void DrawWeeklyCountdown()
        {
            var now = DateTime.UtcNow;
            var delta = Util.NextWeeklyReset() - now;

            ImGui.Text($"Time until weekly reset: {delta.Days} {((delta.Days > 1) ? "days":"day")}, {delta.Hours:00}:{delta.Minutes:00}:{delta.Seconds:00}");
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
            base.OnClose();

            Service.Configuration.Save();
        }

        private void DrawSaveAndCloseButtons()
        {
            ImGui.Spacing();

            ImGui.SetCursorPos(new Vector2(5, ImGui.GetWindowHeight() - 30));

            if (ImGui.Button("Save", new(100, 25)))
            {
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 155);

            if (ImGui.Button("Save & Close", new(150, 25)))
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
