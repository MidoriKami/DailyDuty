using System;
using System.Linq;
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
    internal class DutyRoulette : IConfigurable
    {
        public string ConfigurationPaneLabel => Strings.Module.DutyRouletteLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteInformation);
            }
        };
        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteAutomationInformation);
            }
        };
        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteTechnicalInformation);
            }
        };

        private InfoBox? TodoWindowExpandedInfo { get; } = new()
        {
            Label = Strings.Common.ExpandedInfoLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteExpandedInfo);

                ImGuiHelpers.ScaledDummy(10.0f);

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.ExpandedDisplay))
                {
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, "Expanded Display - " + (Settings.ExpandedDisplay ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox tomeCapped = new()
        {
            Label = Strings.Module.DutyRouletteHideWhenCapped,
            ContentsAction = () =>
            {
                if (ImGui.Checkbox(Strings.Common.EnabledLabel, ref Settings.HideWhenCapped))
                {
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, "Hide When Tomestone Capped - " + (Settings.HideWhenCapped ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
                ImGuiComponents.HelpMarker(Strings.Module.DutyRouletteHideWhenCappedHelp);
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;
        public ModuleType ModuleType => ModuleType.DutyRoulette;
        private static DutyRouletteSettings Settings => Service.CharacterConfiguration.DutyRoulette;

        private readonly InfoBox currentStatus = new()
        {
            Label = Strings.Common.CurrentStatusLabel,
            ContentsAction = () =>
            {
                var numTracked = Settings.TrackedRoulettes.Count(t => t.Tracked);

                if (numTracked == 0)
                {
                    ImGui.TextColored(Colors.Red, Strings.Module.DutyRouletteNothingTrackedDescriptionWarning);
                    ImGui.Text(Strings.Module.DutyRouletteNothingTrackedDescription);
                }
                else
                {
                    if (ImGui.BeginTable($"##Status", 2))
                    {
                        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                        ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                        foreach (var tracked in Settings.TrackedRoulettes)
                        {
                            if (tracked.Tracked)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                var name = tracked.Type.LookupName();
                                ImGui.Text(name[..Math.Min(name.Length, 18)]);

                                ImGui.TableNextColumn();
                                Draw.CompleteIncomplete(tracked.Completed);
                            }
                        }

                        ImGui.EndTable();
                    }
                }
            }
        };

        private readonly InfoBox limitedTomestoneStatus = new()
        {
            Label = Strings.Module.DutyRouletteCurrentTomestomeCount,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<DutyRouletteModule>();
                if (module == null) return;

                if (ImGui.BeginTable($"##TomestoneCount", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);
            
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Common.CountLabel);

                    ImGui.TableNextColumn();
                    var tomeCount = module.GetLimitedTomestoneCount();
                    var limit = module.GetWeeklyTomestomeLimit();

                    ImGui.TextColored(limit != tomeCount ? Colors.Orange : Colors.Green, $"{tomeCount} / {limit}");

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox trackedRoulettes = new()
        {
            Label = Strings.Module.DutyRouletteTrackedRoulettesLabel,
            ContentsAction = () =>
            {
                var contentWidth = ImGui.GetContentRegionAvail().X;

                if (ImGui.BeginTable($"", (int)(contentWidth / 200.0f)))
                {
                    foreach (var roulette in Settings.TrackedRoulettes)
                    {
                        ImGui.TableNextColumn();
                        if (ImGui.Checkbox($"{roulette.Type.LookupName()}", ref roulette.Tracked))
                        {
                            Service.LogManager.LogMessage(ModuleType.DutyRoulette, $"{roulette.Type.LookupName()} " + (roulette.Tracked ? "Enabled" : "Disabled"));
                            Service.CharacterConfiguration.Save();
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
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, Settings.Enabled ? "Enabled" : "Disabled");
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
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox clickableLink = new()
        {
            Label = Strings.Common.ClickableLinkLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.DutyRouletteClickableLinkDescription);

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.EnableClickableLink))
                {
                    Service.LogManager.LogMessage(ModuleType.DutyRoulette, "Clickable Link " + (Settings.EnableClickableLink ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<DutyRouletteModule>();
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

        public DutyRoulette()
        {
            AboutImage = Image.LoadImage("DutyRoulette");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.DutyRouletteLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<DutyRouletteModule>();
                if(module == null) return;

                Draw.CompleteIncompleteRightAligned(module.IsCompleted());
            }
        }
        
        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            trackedRoulettes.DrawCentered();

            DrawExpertRouletteOption();

            ImGuiHelpers.ScaledDummy(30.0f);
            clickableLink.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            TodoWindowExpandedInfo!.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }

        private void DrawExpertRouletteOption()
        {
            var expertRoulette = Settings.TrackedRoulettes
                .Where(t => t.Type == RouletteType.Expert)
                .FirstOrDefault();

            if (expertRoulette is {Tracked: true})
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                tomeCapped.DrawCentered();
            }
        }

        public void DrawStatusContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            completionStatus.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            currentStatus.DrawCentered();

            if (Settings.HideWhenCapped)
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                limitedTomestoneStatus.DrawCentered();
            }

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
