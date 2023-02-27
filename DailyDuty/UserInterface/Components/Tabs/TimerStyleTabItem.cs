using System.Collections.Generic;
using System.Linq;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.UserInterface.Components.Tabs;

public class TimerStyleTabItem : ISelectionWindowTab
{
    public string TabName => "Timer Styles";
    public ISelectable? LastSelection { get; set; }
    
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        if (Service.ConfigurationManager.CharacterConfiguration.HideDisabledTimersInConfigWindow)
        {
            return Service.ModuleManager.GetTimerComponents()
                .Where(timer => timer.ParentModule.GenericSettings.TimerTaskEnabled)
                .Where(module => module.ParentModule.GenericSettings.Enabled)
                .Select(module => new TimerStyleSelectable(module.ParentModule));
        }
        
        return Service.ModuleManager.GetTimerComponents()
            .Select(module => new TimerStyleSelectable(module.ParentModule));
    }

    public void DrawTabExtras()
    {
        DrawHideDisabledButton();
    }
    
    private static void DrawHideDisabledButton()
    {
        var config = Service.ConfigurationManager.CharacterConfiguration;
        
        var region = ImGui.GetContentRegionAvail();

        var label = config.HideDisabledTimersInConfigWindow ? Strings.Config_ShowDisabled : Strings.Config_HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            config.HideDisabledTimersInConfigWindow = !config.HideDisabledTimersInConfigWindow;
            Service.ConfigurationManager.Save();
        }
    }
}