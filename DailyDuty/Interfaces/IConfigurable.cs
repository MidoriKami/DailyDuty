using System;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Utilities;
using DailyDuty.Windows.Settings.Tabs;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface IConfigurable : ICollapsibleHeader
    {
        public GenericSettings GenericSettings { get; }

        void NotificationOptions();

        void EditModeOptions();

        void DisplayData();

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Checkbox($"Enabled", ref GenericSettings.Enabled);
            ImGui.Spacing();

            if (GenericSettings.Enabled)
            {
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                var numColumns = Service.Configuration.System.SingleColumnSettings ? 1 : (int)(ImGui.GetWindowSize().X / 250.0f);

                numColumns = Math.Min(numColumns, ConfigurationTabItem.EditModeEnabled ? 3 : 2);

                if (ImGui.BeginTable($"{HeaderText} Configuration Table)", numColumns))
                {
                    ImGui.TableNextColumn();
                    DisplayData();

                    ImGui.Spacing();

                    if (ConfigurationTabItem.EditModeEnabled)
                    {
                        ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                        ImGui.TableNextColumn();
                        EditModeOptions();
                        ImGui.Spacing();

                        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
                    }

                    ImGui.TableNextColumn();
                    NotificationOptions();
                    ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);

                    ImGui.EndTable();
                }
            }

            ImGui.Spacing();
        }
    }
}