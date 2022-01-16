using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayTabs;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal class DisplayManager : Window
    {
        private Tab currentTab = Tab.ToDo;
        private readonly Vector2 windowSize = new(450, 500);

        private readonly Dictionary<Tab, TabCategory> settingsCategories = new()
        {
            {Tab.ToDo, new ToDoTab()},
            {Tab.Daily, new DailyTab()},
            {Tab.Weekly, new WeeklyTab()}
        };

        public enum Tab
        {
            ToDo,
            Daily,
            Weekly
        }

        public DisplayManager() : base("Daily Duty")
        {
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

            DrawWeeklyCountdown();

            DrawTabs();

            settingsCategories[currentTab].Draw();

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
