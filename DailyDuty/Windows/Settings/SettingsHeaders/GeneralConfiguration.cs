using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Windows.Settings.SettingsHeaders;

internal class GeneralConfiguration : ICollapsibleHeader
{
    public void Dispose()
    {
            
    }

    public string HeaderText { get; } = "General Configuration";
    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

        Draw.NotificationField("Temporary Edit Mode", HeaderText, ref ConfigurationTabItem.EditModeEnabled, 
            "Allows you to manually correct the values stored in each of Daily/Weekly tabs.\n" +
            "Edit Mode automatically disables when you close this window.\n" +
            "Only use Edit Mode to correct errors in other tabs.");

        NotificationDelaySettings();

        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
    }

    private void NotificationDelaySettings()
    {
        ImGui.PushItemWidth(23 * ImGuiHelpers.GlobalScale);
        ImGui.InputInt($"##ZoneChangeDelay{HeaderText}",
            ref Service.Configuration.System.ZoneChangeDelayRate, 0, 0);

        ImGui.PopItemWidth();
        ImGui.SameLine();
        ImGui.Text("Zone Changes Before Resending Notifications");

        ImGuiComponents.HelpMarker("Prevents sending notifications until this many zone changes have happened.\n" +
                                   "1: Notify on Every Zone Change\n" +
                                   "10: Notify on Every 10th Zone change\n" +
                                   "Minimum: 1\n" +
                                   "Maximum: 10");
        ImGui.Spacing();


        if (Service.Configuration.System.ZoneChangeDelayRate < 1)
        {
            Service.Configuration.System.ZoneChangeDelayRate = 1;
        }
        else if (Service.Configuration.System.ZoneChangeDelayRate > 10)
        {
            Service.Configuration.System.ZoneChangeDelayRate = 10;
        }
    }
}