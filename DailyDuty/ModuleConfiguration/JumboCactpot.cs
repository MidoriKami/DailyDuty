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
    internal class JumboCactpot : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.JumboCactpot;
        public string ConfigurationPaneLabel => Strings.Module.JumboCactpotLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.JumboCactpotInformation);
            }
        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.JumboCactpotAutomationInformation);
                ImGui.Spacing();
                ImGui.TextColored(Colors.Orange, Strings.Module.JumboCactpotReSyncInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.JumboCactpotTechnicalInformation);
            }
        };

        private readonly InfoBox completionStatus = new()
        {
            Label = Strings.Common.CompletionStatusLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<JumboCactpotModule>();
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
            Label = Strings.Module.JumboCactpotTicketsLabel,
            ContentsAction = () =>
            {
                var module = Service.ModuleManager.GetModule<JumboCactpotModule>();
                if(module == null) return;

                if (ImGui.BeginTable($"", 2))
                {
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 100f * ImGuiHelpers.GlobalScale);
                    ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 125f * ImGuiHelpers.GlobalScale);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text(Strings.Module.JumboCactpotTicketsLabel);

                    ImGui.TableNextColumn();

                    if (Settings.Tickets.Count == 0)
                    {
                        ImGui.TextColored(Colors.Red, Strings.Common.NoneRecordedLabel);
                    }
                    else
                    {
                        for (var i = 0; i < Settings.Tickets.Count; i++)
                        {
                            ImGui.Text($"[{Settings.Tickets[i]:D4}]");

                            if (i != Settings.Tickets.Count - 1)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                ImGui.TableNextColumn();
                            }
                        }
                    }

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox nextAllowance = new()
        {
            Label = Strings.Module.JumboCactpotNextDrawingLabel,
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
                    ImGui.Text(Strings.Module.JumboCactpotNextDrawingLabel);

                    ImGui.TableNextColumn();
                    var span = Time.NextJumboCactpotReset() - DateTime.UtcNow;
                    ImGui.Text(span.Format());

                    ImGui.EndTable();
                }
            }
        };

        private readonly InfoBox clickableLink = new()
        {
            Label = Strings.Common.ClickableLinkLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Module.JumboCactpotClickableLinkDescription);

                ImGui.Spacing();

                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.EnableClickableLink))
                {
                    Service.LogManager.LogMessage(ModuleType.JumboCactpot, "Clickable Link " + (Settings.EnableClickableLink ? "Enabled" : "Disabled"));
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
                    Service.LogManager.LogMessage(ModuleType.JumboCactpot, Settings.Enabled ? "Enabled" : "Disabled");
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
                    Service.LogManager.LogMessage(ModuleType.JumboCactpot, "Login Notifications " + (Settings.LoginReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }

                if(Draw.Checkbox(Strings.Common.NotifyOnZoneChangeLabel, ref Settings.ZoneChangeReminder, Strings.Common.NotifyOnZoneChangeHelpText))
                {
                    Service.LogManager.LogMessage(ModuleType.JumboCactpot, "Zone Change Notifications " + (Settings.ZoneChangeReminder ? "Enabled" : "Disabled"));
                    Service.CharacterConfiguration.Save();
                }
            }
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.All;

        public static JumboCactpotSettings Settings => Service.CharacterConfiguration.JumboCactpot;


        public JumboCactpot()
        {
            AboutImage = Image.LoadImage("JumboCactpot");
        }

        public void DrawTabItem()
        {
            var moduleName = Strings.Module.JumboCactpotLabel;

            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, moduleName[..Math.Min(moduleName.Length, 22)]);

            if (Settings.Enabled)
            {
                var module = Service.ModuleManager.GetModule<JumboCactpotModule>();
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

            ImGuiHelpers.ScaledDummy(30.0f);
            nextAllowance.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            clickableLink.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            notificationOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
