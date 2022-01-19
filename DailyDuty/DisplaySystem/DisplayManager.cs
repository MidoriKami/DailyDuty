using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.System;
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

            DrawDailyCountdown();
            ImGui.SameLine();
            DrawWeeklyCountdown();

            DrawTabs();

            settingsCategories[currentTab].Draw();

            ImGui.Separator();
            DrawSaveAndCloseButtons();
        }

        private void DrawDailyCountdown()
        {
            var now = DateTime.UtcNow;
            var nextReset = now.AddDays(1).Date.AddHours(15);
            var totalHours = (nextReset - now);
            ImGui.Text($"Time until daily reset: {totalHours.Hours:00}:{totalHours.Minutes:00}:{totalHours.Seconds:00}");
        }

        private void DrawWeeklyCountdown()
        {
            var today = DateTime.UtcNow;
            var nextReset = today.AddDays(1);

            while (nextReset.DayOfWeek != DayOfWeek.Tuesday)
            {
                nextReset = nextReset.AddDays(1);
            }

            var nextResetDate = nextReset.Date.AddHours(8);

            var delta = nextResetDate - today;

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
