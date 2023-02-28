using System.Collections.Generic;
using System.Diagnostics;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components.Tabs;

public class ModuleConfigurationTabItem : ISelectionWindowTab
{
    public string TabName => Strings.Tabs_ModuleConfiguration;
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables() => Service.ModuleManager.GetConfigurationSelectables(filterType);

    private CompletionType? filterType; 

    public void DrawTabExtras()
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(30.0f);
        var region = ImGui.GetContentRegionAvail();
        
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - buttonSize.X / 2.0f;

        ImGui.PushItemWidth(region.X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
        if (ImGui.BeginCombo("##FilterCombo", filterType is null ? Strings.Common_ShowAll : filterType.ToString()))
        {
            if (ImGui.Selectable(Strings.Common_ShowAll, filterType == CompletionType.Daily))
            {
                filterType = null;
            }
            
            if (ImGui.Selectable(Strings.Common_Daily, filterType == CompletionType.Daily))
            {
                filterType = CompletionType.Daily;
            }
            
            if (ImGui.Selectable(Strings.Common_Weekly, filterType == CompletionType.Weekly))
            {
                filterType = CompletionType.Weekly;
            }
            ImGui.EndCombo();
        }
        
        ImGui.SameLine();
        
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);

        if (ImGuiComponents.IconButton("KoFiButton", FontAwesomeIcon.Coffee)) Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/midorikami", UseShellExecute = true });
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Support Me on Ko-Fi");
        
        ImGui.PopStyleColor(3);
    }
}