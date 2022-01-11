using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Reminders.Custom;
using DailyDuty.Reminders.Daily;
using DailyDuty.Reminders.General;
using DailyDuty.Reminders.Weekly;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Reminders
{
    internal class PluginWindow : Window
    {
        private Tab CurrentTab = Tab.General;
        private readonly Vector2 WindowSize = new(450, 500);

        private readonly Dictionary<Tab, TabCategory> settingsCategories = new()
        {
            {Tab.General, new GeneralTab()},
            {Tab.Daily, new DailyTab()},
            {Tab.Weekly, new WeeklyTab()},
            {Tab.Custom, new CustomTab()}
        };

        private enum Tab
        {
            General,
            Daily,
            Weekly,
            Custom
        }

        public PluginWindow() : base("Daily Duty")
        {
            IsOpen = false;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(WindowSize.X, WindowSize.Y),
                MaximumSize = new(WindowSize.X, WindowSize.Y)
            };

            Flags |= ImGuiWindowFlags.NoResize;
            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            DrawDailyCountdown();

            DrawWeeklyCountdown();

            DrawTabs();

            settingsCategories[CurrentTab].Draw();

            ImGui.Separator();
            DrawSaveAndCloseButtons();
        }

        private void DrawDailyCountdown()
        {
            // todo: create daily countdown timer animation
        }

        private void DrawWeeklyCountdown()
        {
            // todo: create weekly countdown timer animation
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("Daily Duty Settings", ImGuiTabBarFlags.NoTooltip))
            {
                foreach (var (tab, data) in settingsCategories)
                {
                    if (ImGui.BeginTabItem(data.TabName))
                    {
                        CurrentTab = tab;
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

            var windowSize = ImGui.GetWindowSize();
            ImGui.SetCursorPos(new Vector2(5, windowSize.Y - 30));

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
