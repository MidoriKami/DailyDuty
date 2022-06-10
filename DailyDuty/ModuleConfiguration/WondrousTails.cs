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

        private readonly InfoBox clickableLink = new()
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
                    Settings.ZoneChangeReminder = Settings.InstanceNotifications || Settings.StickerAvailableNotification;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Instance Notifications " + (Settings.InstanceNotifications ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Module.WondrousTailsStickerAvailableNotificationLabel, ref Settings.StickerAvailableNotification, Strings.Module.WondrousTailsStickerAvailableNotificationDescription))
                {
                    Settings.ZoneChangeReminder = Settings.InstanceNotifications || Settings.StickerAvailableNotification;
                    Service.LogManager.LogMessage(ModuleType.WondrousTails, "Stamp Available Notification " + (Settings.StickerAvailableNotification ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
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
            
                Draw.CompleteIncompleteRightAligned(module.IsCompleted());
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
            clickableLink.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
