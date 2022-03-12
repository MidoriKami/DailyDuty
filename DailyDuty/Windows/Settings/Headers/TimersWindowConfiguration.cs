using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.Graphical;
using DailyDuty.Data.SettingsObjects.Timers;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Timers;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class TimersWindowConfiguration : ICollapsibleHeader
    {
        private TimersWindowSettings Settings => Service.Configuration.Windows.Timers;

        private readonly CountdownTimers countdownTimers;
        public TimersWindowConfiguration()
        {
            var timersList = Service.TimerManager.GetTimers(WindowName.Timers);

            countdownTimers = new CountdownTimers(timersList);
        }

        public void Dispose()
        {
            
        }

        public string HeaderText => "Timers Window Configuration";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ShowHideWindow();

            DisableEnableClickThrough();

            HideInDuty();

            OpacitySlider();

            if (ImGui.BeginTable("TimersWindowConfigurationTable", 8))
            {
                ImGui.TableSetupColumn(" Name", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("  Fg", ImGuiTableColumnFlags.WidthFixed, 25f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("  Bg", ImGuiTableColumnFlags.WidthFixed, 25f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn(" Txt", ImGuiTableColumnFlags.WidthFixed, 25f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn(" Style Options", ImGuiTableColumnFlags.WidthFixed, 200f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn(" Fit", ImGuiTableColumnFlags.WidthFixed, 25f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn(" Size", ImGuiTableColumnFlags.WidthFixed, 175f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                ImGui.TableHeadersRow();

                foreach (var timer in countdownTimers.Timers)
                {
                    DrawTimerSettings(timer);
                }
                
                ImGui.EndTable();
            }

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void DrawTimerSettings(ITimer timer)
        {
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            Draw.Checkbox($"{timer.Name}", ref timer.Settings.Enabled);

            ImGui.TableNextColumn();
            ImGui.ColorEdit4($"##{timer.Name}ForegroundColor", ref timer.Settings.TimerStyle.ForegroundColor, ImGuiColorEditFlags.NoInputs);
        
            ImGui.TableNextColumn();
            ImGui.ColorEdit4($"##{timer.Name}BackgroundColor", ref timer.Settings.TimerStyle.BackgroundColor, ImGuiColorEditFlags.NoInputs);

            ImGui.TableNextColumn();
            ImGui.ColorEdit4($"##{timer.Name}TextColor", ref timer.Settings.TimerStyle.TextColor, ImGuiColorEditFlags.NoInputs);

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(200 * ImGuiHelpers.GlobalScale);

            if (ImGui.BeginCombo($"##{timer.Name}FormatDropdown", timer.Settings.TimerStyle.Options.Format() ?? "Null!", ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var options in TimerOptions.OptionsSamples)
                {
                    bool isSelected = options == timer.Settings.TimerStyle.Options;
                    if (ImGui.Selectable(options.Format(), isSelected))
                    {
                        timer.Settings.TimerStyle.Options = options;
                    }

                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.TableNextColumn();
            Draw.Checkbox($"##{timer.Name}StretchToFit", ref timer.Settings.TimerStyle.StretchToFit);

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(175 * ImGuiHelpers.GlobalScale);
            ImGui.SliderInt($"{timer.Name}Size", ref timer.Settings.TimerStyle.Size, 86, 600);

            ImGui.TableNextColumn();
            if (ImGui.Button($"Reset##{timer.Name}", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                timer.Settings.TimerStyle = new TimerStyle();
                Service.Configuration.Save();
            }
        }
        
        private void OpacitySlider()
        {
            ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.DragFloat($"Opacity", ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
        }

        private void HideInDuty()
        {
            ImGui.Checkbox("Hide when Bound By Duty", ref Settings.HideInDuty);
        }

        private void DisableEnableClickThrough()
        {
            Draw.Checkbox("Enable Click-through", ref Settings.ClickThrough, "Enables/Disables the ability to move the Timers Window");
        }

        private void ShowHideWindow()
        {
            Draw.Checkbox("Show Timers Window", ref Settings.Open, "Shows/Hides the Timers Window");
        }
    }
}
