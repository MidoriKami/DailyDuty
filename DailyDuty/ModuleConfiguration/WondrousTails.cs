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
    internal class WondrousTails : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.WondrousTails;
        public string ConfigurationPaneLabel => Strings.Module.WondrousTailsLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.WondrousTailsInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.WondrousTailsAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.WondrousTailsTechnicalDescription);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<WondrousTailsModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.WondrousTailsBookStatusLabel);

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
                var module = Service.ModuleManager.GetModule<WondrousTailsModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.WondrousTailsNumStamps);

                    ImGui.TableNextColumn();
                    ImGui.TextColored(module.GetNumStamps() == 9 ? Colors.Green : Colors.Orange, module.GetNumStamps() + " / 9");

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox openBookLink = new()
        {
            Label = Strings.Common.ClickableLinkLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.WondrousTailsOpenBookClickableLinkDescription);

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.EnableOpenBookLink))
                {
                    Service.LogManager.LogMessage(ModuleType.DomanEnclave, "Clickable Link " + (Settings.EnableOpenBookLink ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox idyllshireTeleportLink = new()
        {
            Label = Strings.Common.ClickableLinkLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.WondrousTailsIdyllshireTeleportLinkDescription);

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.EnableTeleportLink))
                {
                    Service.LogManager.LogMessage(ModuleType.DomanEnclave, "Teleport Link " + (Settings.EnableTeleportLink ? "Enabled" : "Disabled"));
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
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, Settings.Enabled ? "Enabled" : "Disabled");
                    Service.CharacterConfiguration.Save();
                }

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Module.WondrousTailsInstanceNotificationsLabel, ref Settings.InstanceNotifications, Strings.Module.WondrousTailsInstanceNotificationsDescription))
                {
                    Settings.ZoneChangeReminder = true;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Instance Notifications " + (Settings.InstanceNotifications ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Module.WondrousTailsStickerAvailableNotificationLabel, ref Settings.StickerAvailableNotification, Strings.Module.WondrousTailsStickerAvailableNotificationDescription))
                {
                    Settings.ZoneChangeReminder = true;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Stamp Available Notification " + (Settings.StickerAvailableNotification ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Module.WondrousTailsBookAvailableNotification, ref Settings.UnclaimedBookWarning, Strings.Module.WondrousTailsBookAvailableNotificationDescription))
                {
                    Settings.ZoneChangeReminder = true;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Unclaimed Book Warning " + (Settings.UnclaimedBookWarning ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
                
                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Module.WondrousTailsDeadlineEarlyWarningLabel, ref Settings.DeadlineEarlyWarning, Strings.Module.WondrousTailsDeadlineEarlyWarningDescription))
                {
                    Settings.ZoneChangeReminder = true;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Deadline Early Warning " + (Settings.DeadlineEarlyWarning ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if (Settings.DeadlineEarlyWarning)
                {
                    ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                    ImGui.SetNextItemWidth(50 * ImGuiHelpers.GlobalScale);
                    ImGui.InputInt("", ref Settings.EarlyWarningDays, 0, 0);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        Settings.EarlyWarningDays = Math.Clamp(Settings.EarlyWarningDays, 1, 13);
                        Service.SystemConfiguration.Save();
                    }

                    ImGui.SameLine();
                    ImGui.Text(Strings.Common.DaysLabel);

                    ImGuiComponents.HelpMarker(Strings.Module.WondrousTailsDeadlineEarlyWarningInputDescription);

                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        private static WondrousTailsSettings Settings => Service.CharacterConfiguration.WondrousTails;

        public WondrousTails()
        {
            AboutImage = Image.LoadImage("WondrousTails");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.WondrousTailsLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<WondrousTailsModule>();
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
            openBookLink.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            idyllshireTeleportLink.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
