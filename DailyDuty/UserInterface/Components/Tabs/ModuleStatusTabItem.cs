using System.Collections.Generic;
using System.Linq;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components.Tabs;

public class ModuleStatusTabItem : ISelectionWindowTab
{
    public string TabName => Strings.Tabs_ModuleStatus;
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        if (Service.ConfigurationManager.CharacterConfiguration.HideDisabledModulesInSelectWindow)
        {
            return Service.ModuleManager.GetStatusSelectables()
                .OfType<StatusSelectable>()
                .Where(selectable => selectable.ParentModule.GenericSettings.Enabled);
        }

        return Service.ModuleManager.GetStatusSelectables();
    }

    public void DrawTabExtras()
    {
        DrawHideDisabledButton();
    }
    
    private static void DrawHideDisabledButton()
    {
        var config = Service.ConfigurationManager.CharacterConfiguration;
        
        var region = ImGui.GetContentRegionAvail();

        var label = config.HideDisabledModulesInSelectWindow ? Strings.Config_ShowDisabled : Strings.Config_HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            config.HideDisabledModulesInSelectWindow = !config.HideDisabledModulesInSelectWindow;
            Service.ConfigurationManager.Save();
        }
    }
}