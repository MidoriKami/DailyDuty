using System.Collections.Generic;
using System.Drawing;
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
    public IDrawable Contents => this;

    public ConfigurationSelectable(BaseModule module)
    {
        Module = module;
    }
    
    public void DrawLabel()
    {
        ImGui.Text(Module.ModuleName.GetLabel());
        
        var region = ImGui.GetContentRegionAvail();

        var text = Module.ModuleConfig.ModuleEnabled ? Strings.Enabled : Strings.Disabled;
        var color = Module.ModuleConfig.ModuleEnabled ? KnownColor.ForestGreen : KnownColor.OrangeRed;

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color.AsVector4(), text);
    }
    
    public void Draw()
    {
        Module.DrawConfig();
    }
}