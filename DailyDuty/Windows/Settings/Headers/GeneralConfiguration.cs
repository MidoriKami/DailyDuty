using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Windows.Settings.Tabs;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class GeneralConfiguration : ICollapsibleHeader
    {
        public void Dispose()
        {
            
        }

        public string HeaderText => "General Configuration";

        private SystemSettings System => Service.Configuration.System;
        private SettingsWindowSettings SettingsWindow => Service.Configuration.Windows.Settings;

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            NotificationDelaySettings();

            Draw.Checkbox("Enable Chat Links", HeaderText, ref System.ClickableLinks, "Provides quick-action links for some DailyDuty notifications");

            OpacitySlider();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void OpacitySlider()
        {
            ImGui.Text("Opacity");
            ImGui.SameLine();

            ImGui.PushItemWidth(175 * ImGuiHelpers.GlobalScale);
            ImGui.DragFloat($"##{HeaderText}", ref SettingsWindow.Opacity, 0.01f, 0.0f, 1.0f);
            ImGui.PopItemWidth();
        }

        private void NotificationDelaySettings()
        {
            ImGui.PushItemWidth(23 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##ZoneChangeDelay{HeaderText}",
                ref Service.Configuration.System.ZoneChangeDelayRate, 0, 0);

            ImGui.PopItemWidth();
            ImGui.SameLine();
            ImGui.Text("Zone Changes Before Resending Notifications");

            ImGuiComponents.HelpMarker("Prevents sending notifications until this many zone changes have happened\n" +
                                       "1: Notify on Every Zone Change\n" +
                                       "10: Notify on Every 10th Zone change\n" +
                                       "Minimum: 1\n" +
                                       "Maximum: 10");

            if (Service.Configuration.System.ZoneChangeDelayRate < 1)
            {
                Service.Configuration.System.ZoneChangeDelayRate = 1;
            }
            else if (Service.Configuration.System.ZoneChangeDelayRate > 10)
            {
                Service.Configuration.System.ZoneChangeDelayRate = 10;
            }
        }
    }
}