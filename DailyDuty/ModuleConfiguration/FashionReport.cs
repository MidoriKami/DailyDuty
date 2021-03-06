using System;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Modules;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.ModuleConfiguration
{
    internal class FashionReport : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.FashionReport;
        public string ConfigurationPaneLabel => Strings.Module.FashionReportLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.FashionReportInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.FashionReportAutomationInformation);
                ImGui.Spacing();
                ImGui.TextColored(Colors.Orange, Strings.Module.FashionReportReSyncInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.FashionReportTechnicalInformation);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<FashionReportModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Common.AllTasksLabel);

                    ImGui.TableNextColumn();
                    Draw.CompleteIncomplete(module.IsCompleted());

                    ImGui.EndTable();
                }
            }
        };
        
        private readonly InfoBox modeSelect = new()
        {
            Label = Strings.Module.BeastTribeMarkCompleteWhenLabel,
            ContentsAction = () =>
            {
                var mode = (int) Settings.Mode;

                if (ImGui.BeginTable("ModeSelectTable", 3))
                {
                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.Module.FashionReportSingleModeLabel, ref mode, (int)FashionReportMode.Single);
                    ImGuiComponents.HelpMarker(Strings.Module.FashionReportSingleModeDescription);

                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.Module.FashionReportEightyPlusLabel, ref mode, (int)FashionReportMode.Plus80);
                    ImGuiComponents.HelpMarker(Strings.Module.FashionReportEightyPlusDescription);

                    ImGui.TableNextColumn();
                    ImGui.RadioButton(Strings.Module.FashionReportAllLabel, ref mode, (int)FashionReportMode.All);
                    ImGuiComponents.HelpMarker(Strings.Module.FashionReportAllDescription);

                    ImGui.EndTable();
                }

                if (Settings.Mode != (FashionReportMode) mode)
                {
                    Settings.Mode = (FashionReportMode) mode;
                    Service.LogManager.LogMessage(ModuleType.FashionReport, "Mode Changed - " + Settings.Mode);
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox currentStatus = new()
        {
            Label = Strings.Common.CurrentStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<FashionReportModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.FashionReportAllowancesLabel);

                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.IsCompleted() ? Colors.Green : Colors.Orange, Settings.AllowancesRemaining.ToString());

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.FashionReportHighestScoreLabel);
                    
                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.IsCompleted() ? Colors.Green : Colors.Orange, Settings.HighestWeeklyScore.ToString());

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox nextAllowance = new()
        {
            Label = Strings.Module.FashionReportAvailableLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<FashionReportModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.FashionReportAvailableLabel);

                    ImGui.TableNextColumn();
                    if (module.FashionReportAvailable())
                    {
                        ImGui.TextColored(Colors.Green, Strings.Common.AvailableNowLabel);
                    }
                    else
                    {
                        var span = Time.NextFashionReportReset() - DateTime.UtcNow;
                        ImGui.Text(span.Format());
                    }
                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox clickableLink = new()
        {
            Label = Strings.Common.ClickableLinkLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.FashionReportClickableLinkDescription);

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.EnableClickableLink))
                {
                    Service.LogManager.LogMessage(ModuleType.FashionReport, "Clickable Link " + (Settings.EnableClickableLink ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.LogManager.LogMessage(ModuleType.FashionReport, Settings.Enabled ? "Enabled" : "Disabled");
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox notificationOptions = new()
        {
            Label = Strings.Common.NotificationOptionsLabel,
            ContentsAction = () =>
            {
                if(Draw.Checkbox(Strings.Common.NotifyOnLoginLabel, ref Settings.LoginReminder, Strings.Common.NotifyOnLoginHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.FashionReport, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.FashionReport, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private static FashionReportSettings Settings => Service.CharacterConfiguration.FashionReport;
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        public FashionReport()
        {
            AboutImage = Image.LoadImage("FashionReport");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.FashionReportLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<FashionReportModule>();
                if(module == null) return;

                if(module.FashionReportAvailable())
                {
                    Draw.CompletionStatus(module.IsCompleted());
                }
                else
                {
                    Draw.CompletionStatus(CompletionStatus.Unavailable);
                }
            }
        }

        public void DrawStatusContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            completionStatus.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            currentStatus.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            nextAllowance.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            modeSelect.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            clickableLink.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
