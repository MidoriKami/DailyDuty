using System.Globalization;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Modules;
using DailyDuty.Structs;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.ModuleConfiguration
{
    internal class HuntMarksDaily : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.HuntMarksDaily;
        public string ConfigurationPaneLabel => Strings.Module.HuntMarksDailyLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksDailyInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.HuntMarksTechnicalInformation);
            }
        };
        
        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<HuntMarksDailyModule>();
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
                if (Settings.TrackedHunts.Where(hunt => hunt.Tracked).Count() == 0)
                {
                    ImGui.Text(Strings.Module.HuntMarksNoHuntsTracked);
                }
                else if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    foreach (var hunt in Settings.TrackedHunts.Where(hunt => hunt.Tracked))
                    {
                        var label = hunt.Type.GetLabel();

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
                    Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, Settings.Enabled ? "Enabled" : "Disabled");
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox trackingSettings = new()
        {
            Label = Strings.Module.HuntMarksTrackedHuntsLabel,
            ContentsAction = () =>
            {
                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 150f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();

                    DrawTrackUntrackButton();

                    foreach (var hunt in Settings.TrackedHunts)
                    {
                        var label = hunt.Type.GetLabel();

                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        ImGui.Text(label);

                        ImGui.TableNextColumn();
                        if (ImGui.Checkbox($"{Strings.Module.HuntMarksTrackLabel}##{hunt.Type}", ref hunt.Tracked))
                        {
                            Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, label + (hunt.Tracked ? " Tracked" : " Untracked"));
                            Service.CharacterConfiguration.Save();
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        private static void DrawTrackUntrackButton()
        {
            if (Settings.TrackedHunts.All(hunt => hunt.Tracked))
            {
                if (ImGui.Button(Strings.Module.HuntMarksUntrackAllLabel, ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
                {
                    Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, "Untrack All Selected");

                    foreach (var hunt in Settings.TrackedHunts)
                    {
                        hunt.Tracked = false;
                    }
                }
            }
            else
            {
                if (ImGui.Button(Strings.Module.HuntMarksTrackAllLabel, ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
                {
                    Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, "Track All Selected");

                    foreach (var hunt in Settings.TrackedHunts)
                    {
                        hunt.Tracked = true;
                    }
                }
            }
        }

        private readonly InfoBox notificationOptions = new()
        {
            Label = Strings.Common.NotificationOptionsLabel,
            ContentsAction = () =>
            {
                if(Draw.Checkbox(Strings.Common.NotifyOnLoginLabel, ref Settings.LoginReminder, Strings.Common.NotifyOnLoginHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.HuntMarksDaily, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;


        private static DailyHuntMarksSettings Settings => Service.CharacterConfiguration.DailyHuntMarks;

        public HuntMarksDaily()
        {
            AboutImage = Image.LoadImage("DailyHuntMarks");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Module.HuntMarksDailyLabel);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<HuntMarksDailyModule>();
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
            DrawTargetInfo();

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

        private unsafe void DrawTargetInfo()
        {
            var module = Service.ModuleManager.GetModule<HuntMarksWeeklyModule>();
            if(module == null) return;

            foreach (var hunt in Settings.TrackedHunts.Where(hunt => hunt.Tracked && hunt.State == TrackedHuntState.Obtained).GroupBy(hunt => hunt.Type.GetExpansion()))
            {
                new InfoBox()
                {
                    Label = hunt.Key.GetLabel(),
                    ContentsAction = () =>
                    {
                        foreach (var specificHunt in hunt)
                        {
                            ImGui.Text(specificHunt.Type.GetLevel());

                            var huntData = module.HuntData->Get(specificHunt.Type);

                            DrawHuntInfoTable(huntData);

                            ImGuiHelpers.ScaledDummy(5.0f);

                            DrawTeleportButton(specificHunt, huntData);

                            ImGuiHelpers.ScaledDummy(5.0f);
                        }
                    }

                }.DrawCentered();

                ImGuiHelpers.ScaledDummy(30.0f);
            }
        }

        private void DrawTeleportButton(TrackedHunt specificHunt, HuntData huntData)
        {
            var cursorStart = ImGui.GetCursorPos();
            var buttonSize = ImGuiHelpers.ScaledVector2(100.0f, 23.0f);
            ImGui.SetCursorPos(cursorStart with
            {
                X = cursorStart.X + 100.0f * ImGuiHelpers.GlobalScale - buttonSize.X / 2.0f
            });

            if (ImGui.Button(Strings.Common.TeleportLabel + $"###{specificHunt.Type}", buttonSize))
            {
                var targetID = GetFirstIncomplete(huntData)?.Target.Value?.TerritoryType.Value?.TerritoryType.Value?.RowId;
                Service.TeleportManager.Teleport(targetID);
            }
        }

        private void DrawHuntInfoTable(HuntData huntData)
        {
            if (ImGui.BeginTable($"", 2))
            {
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);
                ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 175f * ImGuiHelpers.GlobalScale);

                ImGui.TableNextRow();
                DrawTargetName(huntData);

                ImGui.TableNextRow();
                DrawTargetLocation(huntData);

                ImGui.EndTable();
            }
        }

        private void DrawTargetLocation(HuntData huntData)
        {
            ImGui.TableNextColumn();
            ImGui.Text(Strings.Module.HuntMarksTargetLocation);

            ImGui.TableNextColumn();
            ImGui.Text(GetFirstIncomplete(huntData)?.Target.Value?.TerritoryType.Value?.PlaceName.Value?.Name ??
                       "Unable to Read Table");
        }

        private void DrawTargetName(HuntData huntData)
        {
            ImGui.TableNextColumn();
            ImGui.Text(Strings.Module.HuntMarksTargetName);ImGui.TableNextColumn();
            var name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
                GetFirstIncomplete(huntData)?.Target.Value?.Name.Value?.Singular ?? "Unable to Read Table");
            ImGui.Text(name);
            var currentKills = GetFirstIncompleteCurrentKillCount(huntData);
            var neededKills = GetFirstIncomplete(huntData)?.NeededKills;
            ImGui.SameLine();
            ImGui.Text($"{currentKills}/{neededKills}");
        }

        private MobHuntOrder? GetFirstIncomplete(HuntData data)
        {
            var targetInfo = data.TargetInfo;

            for (int i = 0; i < 5; ++i)
            {
                if (targetInfo[i].NeededKills > data.KillCounts[i])
                    return targetInfo[i];
            }

            return null;
        }

        private int GetFirstIncompleteCurrentKillCount(HuntData data)
        {
            var targetInfo = data.TargetInfo;

            for (int i = 0; i < 5; ++i)
            {
                if (targetInfo[i].NeededKills > data.KillCounts[i])
                    return data.KillCounts[i];
            }

            return 0;
        }
    }
}
