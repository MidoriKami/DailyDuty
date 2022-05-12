using System.Linq;
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
                ImGui.DragFloat(Strings.Common.OpacityLabel, ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
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

        private static TimerStyle _newConfiguration = new();

        private readonly InfoBox applyAll = new()
        {
            Label = Strings.Common.ApplyAllLabel,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(200.0f * ImGuiHelpers.GlobalScale);
                if (ImGui.BeginCombo($"{Strings.Common.StyleLabel}##ApplyAllFormatDropdown", _newConfiguration.Options.Format(), ImGuiComboFlags.PopupAlignLeft))
                {
                    foreach (var optionsPreset in TimerOptions.OptionsSamples)
                    {
                        if (ImGui.Selectable(optionsPreset.Format(), optionsPreset == _newConfiguration.Options))
                        {
                            _newConfiguration.Options = optionsPreset;
                        }
                    }

                    ImGui.EndCombo();
                }

                if (ImGui.Button(Strings.Common.ApplyAllLabel, ImGuiHelpers.ScaledVector2(80, 25)))
                {
                    foreach (var timer in Service.TimerManager.Timers.Where(t => t.TimerSettings.Enabled))
                    {
                        timer.TimerSettings.TimerStyle.Options = _newConfiguration.Options;
                    }
                    Service.SystemConfiguration.Save();
                }

                ImGui.SameLine();

                if (ImGui.Button(Strings.Common.ResetAllLabel, ImGuiHelpers.ScaledVector2(80, 25)))
                {
                    foreach (var timer in Service.TimerManager.Timers.Where(t => t.TimerSettings.Enabled))
                    {
                        timer.TimerSettings.TimerStyle.Options = new TimerOptions();
                    }
                    Service.SystemConfiguration.Save();
                }
            }
        };

        public InfoBox? AutomationInformationBox => null;
        public InfoBox? TechnicalInformation => null;
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

            ImGuiHelpers.ScaledDummy(30.0f);
            applyAll.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
            DrawTimersConfiguration();

            ImGuiHelpers.ScaledDummy(20.0f);
        }


        private void DrawTimersConfiguration()
        {
            foreach (var timer in Service.TimerManager.Timers.Where(t => t.TimerSettings.Enabled))
            {
                ImGuiHelpers.ScaledDummy(10.0f);

                new InfoBox
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

            ImGui.SameLine();
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(175 * ImGuiHelpers.GlobalScale);
            ImGui.SliderInt(Strings.Features.TimersWindowSizeLabel + $"##{timer.Label}", ref timer.TimerSettings.TimerStyle.Size, 86, 600);
            if(ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.SystemConfiguration.Save();
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
