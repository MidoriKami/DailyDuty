using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Timers;
using DailyDuty.Windows.Settings.Tabs;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Settings
{
    internal class SettingsWindow : Window, IWindow
    {

        private readonly CountdownTimers countdownTimers;
        private readonly SaveAndCloseButtons saveAndCloseButtons;
        public new WindowName WindowName => WindowName.Settings;

        private readonly List<ITabItem> tabs = new()
        {
            new OverviewTabItem(),
            new DailyTabItem(),
            new WeeklyTabItem(),
            new ConfigurationTabItem()
        };
        private SettingsWindowSettings Settings => Service.Configuration.Windows.Settings;

        public SettingsWindow() : base("DailyDuty Settings")
        {
            saveAndCloseButtons = new(this);

            Service.WindowSystem.AddWindow(this);

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(265, 250),
                MaximumSize = new(9999,9999)
            };

            var timersList = Service.TimerManager.GetTimers(WindowName.Settings);

            countdownTimers = new CountdownTimers(timersList);

            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public void Dispose()
        {
            foreach (var tab in tabs)
            {
                tab.Dispose();
            }

            Service.WindowSystem.RemoveWindow(this);
        }

        public override void PreOpenCheck()
        {
            if (Service.LoggedIn == false)
            {
                IsOpen = false;
            }
        }

        public override void PreDraw()
        {
            var color = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(color.X, color.Y, color.Z, Settings.Opacity));

            color = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(color.X, color.Y, color.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(10, 5));

            countdownTimers.Draw();

            ImGui.Spacing();

            DrawTabs();

            saveAndCloseButtons.Draw();

            ImGui.PopStyleVar();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(2);
        }

        private void DrawTabs()
        {
            if (ImGui.BeginTabBar("DailyDutyTabBar", ImGuiTabBarFlags.NoTooltip))
            {
                foreach (var tab in tabs)
                {
                    if (!ImGui.BeginTabItem(tab.TabName))
                        continue;

                    var height = ImGui.GetContentRegionAvail().Y - 30 * ImGuiHelpers.GlobalScale;

                    if (ImGui.BeginChild("DailyDutySettings", new Vector2(0, height), true)) 
                    {
                        tab.Draw();
                        ImGui.EndChild();
                    }

                    ImGui.EndTabItem();
                }
            }
        }

        public override void OnClose()
        {
            ConfigurationTabItem.EditModeEnabled = false;
        }
    }
}