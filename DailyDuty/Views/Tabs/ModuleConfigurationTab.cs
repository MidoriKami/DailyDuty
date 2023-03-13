using System.Collections.Generic;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using DailyDuty.System.Localization;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class ModuleConfigurationTab : ISelectionWindowTab
{
    public string TabName => Strings.ModuleConfiguration;
    public ISelectable? LastSelection { get; set; }
    
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        return DailyDutyPlugin.System.ModuleController
            .GetModules()
            .Select(module => new ConfigurationSelectable(module));
    }
}

public class ConfigurationSelectable : ISelectable, IDrawable
{
    public BaseModule Module;
    public string ID => Module.ModuleName.ToString();

    public ConfigurationSelectable(BaseModule module)
    {
        Module = module;
    }
    public IDrawable Contents => this;
    
    public void DrawLabel()
    {
        ImGui.Text(Module.ModuleName.GetLabel());
    }
    
    public void Draw()
    {
        Module.DrawConfig();
    }
}