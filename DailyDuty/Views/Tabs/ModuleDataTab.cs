using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class ModuleDataTab :ISelectionWindowTab
{
    public string TabName => "Module Data";
    public ISelectable? LastSelection { get; set; }
    public IEnumerable<ISelectable> GetTabSelectables()
    {       
        return DailyDutyPlugin.System.ModuleController
            .GetModules()
            .Select(module => new DataSelectable(module));
    }
}

public class DataSelectable : ISelectable, IDrawable
{
    private BaseModule Module;
    public IDrawable Contents => this;
    public string ID => Module.ModuleName.ToString();

    public DataSelectable(BaseModule module)
    {
        Module = module;
    }
    
    public void DrawLabel()
    {
        ImGui.Text(Module.ModuleName.GetLabel());

        var region = ImGui.GetContentRegionAvail();

        var color = Module.ModuleStatus.GetColor();
        var text = Module.ModuleStatus.GetLabel();

        // Override Status if Module is Disabled
        if (!Module.ModuleConfig.ModuleEnabled)
        {
            text = "Disabled";
            color = KnownColor.Gray.AsVector4();
        }

        var textSize = ImGui.CalcTextSize(text);

        ImGui.SameLine(region.X - textSize.X + 3.0f);
        ImGui.TextColored(color, text);
    }
    
    public void Draw()
    {
        Module.DrawData();
    }
}