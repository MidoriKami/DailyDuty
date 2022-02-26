using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class TodoWindowConfiguration : ICollapsibleHeader
    {
        private TodoWindowSettings Settings => Service.Configuration.Windows.Todo;

        public string HeaderText => "Todo Window Configuration";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ShowHideWindow();

            DisableEnableClickThrough();

            TaskSelection();

            HideInDuty();

            HideWhenComplete();

            ShowWhenComplete();

            OpacitySlider();

            EditColors();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void ShowWhenComplete()
        {
            Draw.Checkbox("Show Completed Tasks", HeaderText, ref Settings.ShowTasksWhenComplete, "Show all tracked tasks, using complete/incomplete colors");
        }

        public void Dispose()
        {
            
        }

        private void OpacitySlider()
        {
            ImGui.PushItemWidth(150);
            ImGui.DragFloat($"Opacity##{HeaderText}", ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
            ImGui.PopItemWidth();
        }

        private void HideWhenComplete()
        {
            Draw.Checkbox("Hide Completed Categories", HeaderText, ref Settings.HideWhenTasksComplete);
        }

        private void HideInDuty()
        {
            ImGui.Checkbox("Hide when Bound By Duty", ref Settings.HideInDuty);
        }

        private void TaskSelection()
        {
            Draw.Checkbox("Daily Tasks", HeaderText, ref Settings.ShowDaily, "Show/Hide Daily Tasks category");

            Draw.Checkbox("Weekly Tasks", HeaderText, ref Settings.ShowWeekly, "Show/Hide Weekly Tasks category");
        }

        private void DisableEnableClickThrough()
        {
            Draw.Checkbox("Enable Click-through", HeaderText, ref Settings.ClickThrough, "Enables/Disables the ability to move the Todo Window");
        }

        private void ShowHideWindow()
        {
            Draw.Checkbox("Show Todo Window", HeaderText, ref Settings.Open, "Shows/Hides the Todo Window");
        }

        private void EditColors()
        {
            ImGui.ColorEdit4("Header Color", ref Settings.Colors.HeaderColor, ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Completed Task Color", ref Settings.Colors.CompleteColor, ImGuiColorEditFlags.NoInputs);
            ImGui.ColorEdit4("Incomplete Task Color", ref Settings.Colors.IncompleteColor, ImGuiColorEditFlags.NoInputs);
        }
    }
}