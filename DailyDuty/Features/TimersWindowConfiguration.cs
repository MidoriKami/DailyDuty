using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Timers;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Features
{
    internal class TimersWindowConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TimersWindow;

        private static readonly Stopwatch settingsStopwatch = new();
        public string ConfigurationPaneLabel => Strings.Features.TimersWindowLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () => { ImGui.Text(Strings.Features.TimersWindowInformation); }
        };

        private readonly InfoBox options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.SystemConfiguration.Save();
                }

                if (Draw.Checkbox(Strings.Common.ClickthroughLabel, ref Settings.ClickThrough))
                {
                    Service.SystemConfiguration.Save();
                }

                ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
                if (ImGui.DragFloat(Strings.Common.OpacityLabel, ref Settings.Opacity, 0.01f, 0.0f, 1.0f))
                {
                    settingsStopwatch.Restart();
                }

                if (settingsStopwatch.ElapsedMilliseconds > 500)
                {
                    settingsStopwatch.Reset();
                    Service.SystemConfiguration.Save();
                }
            }
        };

        private readonly InfoBox windowHiding = new()
        {
            Label = Strings.Features.TodoWindowHideWindowWhenLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Features.TodoWindowHideInDutyLabel, ref Settings.HideInDuty))
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        private readonly InfoBox enableTimers = new()
        {
            Label = Strings.Features.TimersWindowTimersEnableLabel,
            ContentsAction = () =>
            {
                var region = ImGui.GetContentRegionAvail();

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    foreach (var timer in Service.TimerManager.Timers)
                    {
                        var label = timer.Label;

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(label);

                        ImGui.TableNextColumn();
                        if (ImGui.Checkbox($"{Strings.Common.EnabledLabel}##{label}", ref timer.TimerSettings.Enabled))
                        {
                            Service.SystemConfiguration.Save();
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        public InfoBox? AutomationInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;

        private static TimersWindowSettings Settings => Service.SystemConfiguration.Windows.Timers;

        public TimersWindowConfiguration()
        {
            AboutImage = Image.LoadImage("TimersWindow");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Features.TimersWindowLabel);

        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            enableTimers.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            windowHiding.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(20.0f);
            DrawTimersConfiguration();

            ImGuiHelpers.ScaledDummy(20.0f);
        }


        private void DrawTimersConfiguration()
        {
            foreach (var timer in Service.TimerManager.Timers.Where(t => t.TimerSettings.Enabled))
            {
                ImGuiHelpers.ScaledDummy(10.0f);

                new InfoBox()
                {
                    Label = timer.Label,
                    ContentsAction = () =>
                    {
                        DrawTimer(timer);
                    }

                }.DrawCentered();

                ImGuiHelpers.ScaledDummy(10.0f);
            }
            
        }

        private static void DrawTimer(CountdownTimer timer)
        {
            DrawColorOptions(timer);

            DrawFitToWindowOptions(timer);

            DrawFormattingOptions(timer);

            if (ImGui.Button($"{Strings.Common.ResetLabel}##{timer.Label}", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                timer.TimerSettings.TimerStyle = new TimerStyle();
                Service.SystemConfiguration.Save();
            }

            if (settingsStopwatch.ElapsedMilliseconds > 500)
            {
                settingsStopwatch.Reset();
                Service.SystemConfiguration.Save();
            }
        }

        private static void DrawFormattingOptions(CountdownTimer timer)
        {
            ImGui.SetNextItemWidth(200.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginCombo($"{Strings.Common.StyleLabel}##{timer.Label}FormatDropdown", timer.TimerSettings.TimerStyle.Options.Format(), ImGuiComboFlags.PopupAlignLeft))
            {
                foreach (var options in TimerOptions.OptionsSamples)
                {
                    if (ImGui.Selectable(options.Format(), options == timer.TimerSettings.TimerStyle.Options))
                    {
                        timer.TimerSettings.TimerStyle.Options = options;
                        Service.SystemConfiguration.Save();
                    }
                }

                ImGui.EndCombo();
            }
        }

        private static void DrawFitToWindowOptions(CountdownTimer timer)
        {
            if (Draw.Checkbox(Strings.Features.TimersWindowFitToWindowLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.StretchToFit))
            {
                Service.SystemConfiguration.Save();
            }

            if (!timer.TimerSettings.TimerStyle.StretchToFit)
            {
                ImGui.SameLine();
                ImGui.TableNextColumn();
                ImGui.SetNextItemWidth(175 * ImGuiHelpers.GlobalScale);
                if (ImGui.SliderInt(Strings.Features.TimersWindowSizeLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.Size, 86, 600))
                {
                    settingsStopwatch.Restart();
                }
            }
        }

        // Draws as a 2x2 grid
        private static void DrawColorOptions(CountdownTimer timer)
        {
            if (ImGui.ColorEdit4(Strings.Features.TimersWindowForegroundColorLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.ForegroundColor, ImGuiColorEditFlags.NoInputs))
            {
                Service.SystemConfiguration.Save();
            }

            ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);

            if (ImGui.ColorEdit4(Strings.Features.TimersWindowBackgroundColorLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.BackgroundColor, ImGuiColorEditFlags.NoInputs))
            {
                Service.SystemConfiguration.Save();
            }

            if (ImGui.ColorEdit4(Strings.Features.TimersWindowTextColorLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.TextColor, ImGuiColorEditFlags.NoInputs))
            {
                Service.SystemConfiguration.Save();
            }

            ImGui.SameLine(150.0f * ImGuiHelpers.GlobalScale);

            if (ImGui.ColorEdit4(Strings.Features.TimersWindowTimeColorLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.TimeColor, ImGuiColorEditFlags.NoInputs))
            {
                Service.SystemConfiguration.Save();
            }
        }
    }
}
