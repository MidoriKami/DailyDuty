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
    internal class CustomDelivery : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.CustomDelivery;
        public string ConfigurationPaneLabel => Strings.Module.CustomDeliveryLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.CustomDeliveryInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.CustomDeliveryAutomationInformation);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<CustomDeliveryModule>();
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
                var contentWidth = ImGui.GetContentRegionAvail();

                ImGui.SetNextItemWidth(contentWidth.X * 0.40f);
                if (ImGui.BeginCombo("", Settings.ComparisonMode.GetLabel()))
                {
                    foreach (var value in Enum.GetValues<ComparisonMode>())
                    {
                        if (ImGui.Selectable(value.GetLabel(), Settings.ComparisonMode == value))
                        {
                            Service.LogManager.LogMessage(ModuleType.CustomDelivery, "Comparison Mode - " + value);
                            Settings.ComparisonMode = value;
                            Service.CharacterConfiguration.Save();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();
                
                ImGui.SetNextItemWidth(contentWidth.X * 0.20f);
                ImGui.SliderInt(Strings.Common.AllowancesLabel, ref Settings.NotificationThreshold, 0, 12);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Service.LogManager.LogMessage(ModuleType.CustomDelivery, "Threshold - " + Settings.NotificationThreshold);
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox currentStatus = new()
        {
            Label = Strings.Common.CurrentStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<CustomDeliveryModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.CustomDeliveryAllowancesLabel);

                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.IsCompleted() ? Colors.Green : Colors.Orange, module.GetRemainingAllowances().ToString());

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
                    Service.LogManager.LogMessage(ModuleType.CustomDelivery, Settings.Enabled ? "Enabled" : "Disabled");
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
                    Service.LogManager.LogMessage(ModuleType.CustomDelivery, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.CustomDelivery, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public InfoBox? TechnicalInformation => null;
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        private static CustomDeliverySettings Settings => Service.CharacterConfiguration.CustomDelivery;


        public CustomDelivery()
        {
            AboutImage = Image.LoadImage("CustomDelivery");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.CustomDeliveryLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<CustomDeliveryModule>();
                if(module == null) return;

                Draw.CompletionStatus(module.IsCompleted());
            }
        }

        public void DrawStatusContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            completionStatus.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            currentStatus.DrawCentered();

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
