using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
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

            Draw.Checkbox("Enable Chat Links", ref System.ClickableLinks, "Provides quick-action links for some DailyDuty notifications");

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
            ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);
            ImGui.InputInt($"##ZoneChangeDelay{HeaderText}",
                ref Service.Configuration.System.MinutesBetweenThrottledMessages, 0, 0);

            ImGui.PopItemWidth();
            ImGui.SameLine();
            ImGui.Text("Minutes Before Resending Notifications");

            ImGuiComponents.HelpMarker("Prevents sending notifications until this many minutes have elapsed\n" +
                                       "Minimum:  1 Minute\n" +
                                       "Maximum: 60 Minutes");

            if (Service.Configuration.System.MinutesBetweenThrottledMessages < 1)
            {
                Service.Configuration.System.MinutesBetweenThrottledMessages = 1;
            }
            else if (Service.Configuration.System.MinutesBetweenThrottledMessages > 60)
            {
                Service.Configuration.System.MinutesBetweenThrottledMessages = 60;
            }
        }
    }
}