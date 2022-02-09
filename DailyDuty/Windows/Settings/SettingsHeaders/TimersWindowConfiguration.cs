using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects.WindowSettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.SettingsHeaders
{
    internal class TimersWindowConfiguration : ICollapsibleHeader
    {
        private TimersWindowSettings Settings => Service.Configuration.TimersWindowSettings;
        public string HeaderText => "Timers Window Configuration";

        public void Dispose()
        {
        }

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ShowHideWindow();

            DisableEnableClickThrough();

            HideInDuty();

            OpacitySlider();
            
            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void OpacitySlider()
        {
            ImGui.PushItemWidth(150);
            ImGui.DragFloat($"Opacity##{HeaderText}", ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
            ImGui.PopItemWidth();
        }

        private void HideInDuty()
        {
            ImGui.Checkbox("Hide when Bound By Duty", ref Settings.HideInDuty);
        }

        private void DisableEnableClickThrough()
        {
            Draw.NotificationField("Enable Click-through", HeaderText, ref Settings.ClickThrough, "Enables/Disables the ability to move the Timers Window");
        }

        private void ShowHideWindow()
        {
            Draw.NotificationField("Show Timers Window", HeaderText, ref Settings.Open, "Shows/Hides the Timers Window");
        }
    }
}
