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
    internal class HuntMarksWeekly : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.HuntMarksWeekly;
        public string ConfigurationPaneLabel => Strings.Module.HuntMarksWeeklyLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksWeeklyInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksWeeklyAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksWeeklyTechnicalInformation);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<HuntMarksWeeklyModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

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
                var module = Service.ModuleManager.GetModule<HuntMarksWeeklyModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    foreach (var hunt in Settings.TrackedHunts)
                    {
                        var label = module.GetExpansionForHuntType(hunt.Type).Description();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(label);

                        ImGui.TableNextColumn();
                        switch (hunt.State)
                        {
                            case TrackedHuntState.Unobtained:
                                ImGui.TextColored(Colors.Red, Strings.Module.HuntMarksMarkAvailableLabel);
                                break;
                            case TrackedHuntState.Obtained:
                                ImGui.TextColored(Colors.Orange, Strings.Module.HuntMarksMarkObtainedLabel);
                                break;
                            case TrackedHuntState.Killed:
                                ImGui.TextColored(Colors.Green, Strings.Module.HuntMarksMarkKilledLabel);
                                break;
                        }
                    }

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
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, Settings.Enabled ? "Enabled" : "Disabled");
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox trackingSettings = new()
        {
            Label = Strings.Module.HuntMarksTrackedHuntsLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<HuntMarksWeeklyModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    foreach (var hunt in Settings.TrackedHunts)
                    {
                        var label = module.GetExpansionForHuntType(hunt.Type).Description();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(label);

                        ImGui.TableNextColumn();
                        if (ImGui.Checkbox($"{Strings.Module.HuntMarksTrackLabel}##{hunt.Type}", ref hunt.Tracked))
                        {
                            Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, label + (hunt.Tracked ? " Tracked" : " Untracked"));
                            Service.CharacterConfiguration.Save();
                        }
                    }

                    ImGui.EndTable();
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
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, "Login Notifications " + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.HuntMarksWeekly, "Zone Change Notifications " + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        private static WeeklyHuntMarksSettings Settings => Service.CharacterConfiguration.WeeklyHuntMarks;

        public HuntMarksWeekly()
        {
            AboutImage = Image.LoadImage("WeeklyHuntMarks");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Module.HuntMarksWeeklyLabel);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<HuntMarksWeeklyModule>();
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
            trackingSettings.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
