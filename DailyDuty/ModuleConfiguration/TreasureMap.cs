using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    internal class TreasureMap : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TreasureMap;
        public string ConfigurationPaneLabel => Strings.Module.TreasureMapLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.TreasureMapInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.TreasureMapAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation => null;
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        private readonly InfoBox options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.LogManager.LogMessage(ModuleType.TreasureMap, Settings.Enabled ? "Enabled" : "Disabled");
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
                    Service.LogManager.LogMessage(ModuleType.TreasureMap, "Login Notifications " + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.TreasureMap, "Zone Change Notifications " + (Settings.Enabled ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<TreasureMapModule>();
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

        private readonly InfoBox mapStatus = new()
        {
            Label = Strings.Module.TreasureMapStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<TreasureMapModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 125f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed, 100f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.TreasureMapTimeUntilNextMap);


                    ImGui.TableNextColumn();
                    var span = module.TimeUntilNextMap();
                    if (span == TimeSpan.Zero)
                    {
                        ImGui.TextColored(new(0, 255, 0, 255), $" {span.Format()}");
                    }
                    else
                    {
                        ImGui.Text($" {span.Format()}");
                    }

                    ImGui.EndTable();
                }
            }
        };

        private static TreasureMapSettings Settings => Service.CharacterConfiguration.TreasureMap;

        public TreasureMap()
        {
            AboutImage = Image.LoadImage("TreasureMap");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Module.TreasureMapLabel);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }

        public void DrawStatusContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            completionStatus.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);
            mapStatus.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
