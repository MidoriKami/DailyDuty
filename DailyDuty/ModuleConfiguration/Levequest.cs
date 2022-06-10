using System;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Modules;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.ModuleConfiguration
{
    internal class Levequest : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.Levequest;
        public string ConfigurationPaneLabel => Strings.Module.LevequestLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.LevequestInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.LevequestAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.LevequestTechnicalInformation);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<LevequestModule>();
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

        private readonly InfoBox currentStatus = new()
        {
            Label = Strings.Common.CurrentStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<LevequestModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Common.AllowancesLabel);

                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.IsCompleted() ? Colors.Green : Colors.Orange, module.GetRemainingAllowances().ToString());

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.LevequestAcceptedLabel);
                    
                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.IsCompleted() ? Colors.Green : Colors.Orange, module.GetAcceptedLeves().ToString());

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox nextAllowance = new()
        {
            Label = Strings.Module.NextAllowanceLabel,
            ContentsAction = () =>
            {
                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.NextAllowanceLabel);

                    ImGui.TableNextColumn();
                    var span = Time.NextLeveAllowanceReset() - DateTime.UtcNow;
                    ImGui.Text(span.Format());

                    ImGui.EndTable();
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
                    Service.LogManager.LogMessage(ModuleType.Levequest, Settings.Enabled ? "Enabled" : "Disabled");
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
                    Service.LogManager.LogMessage(ModuleType.Levequest, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.Levequest, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox modeSelect = new()
        {
            Label = Strings.Module.BeastTribeMarkCompleteWhenLabel,
            ContentsAction = () =>
            {
                var contentWidth = ImGui.GetContentRegionAvail();

                ImGui.SetNextItemWidth(contentWidth.X * 0.40f);
                if (ImGui.BeginCombo("", Settings.ComparisonMode.GetLabel()))
                {
                    foreach (var value in Enum.GetValues<ComparisonMode>())
                    {
                        if (ImGui.Selectable(value.GetLabel(), Settings.ComparisonMode == value))
                        {
                            Service.LogManager.LogMessage(ModuleType.Levequest, "Comparison Mode - " + value);
                            Settings.ComparisonMode = value;
                            Service.CharacterConfiguration.Save();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();
                
                ImGui.SetNextItemWidth(contentWidth.X * 0.20f);
                ImGui.SliderInt(Strings.Common.AllowancesLabel, ref Settings.NotificationThreshold, 0, 100);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Service.LogManager.LogMessage(ModuleType.Levequest, "Threshold - " + Settings.NotificationThreshold);
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        private static LevequestSettings Settings => Service.CharacterConfiguration.Levequest;

        public Levequest()
        {
            AboutImage = Image.LoadImage("Levequest");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.LevequestLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<LevequestModule>();
                if(module == null) return;

                Draw.CompleteIncompleteRightAligned(module.IsCompleted());
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
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
