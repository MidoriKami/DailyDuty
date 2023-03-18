using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class ModuleConfigurationTab : ISelectionWindowTab
{
    public string TabName => Strings.ModuleConfiguration;
    public ISelectable? LastSelection { get; set; }
    
    private ModuleType? filterType; 
    
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        return DailyDutyPlugin.System.ModuleController
            .GetModules(filterType)
            .Select(module => new ConfigurationSelectable(module));
    }

    public void DrawTabExtras()
    {
        var buttonSize = ImGuiHelpers.ScaledVector2(30.0f);
        var region = ImGui.GetContentRegionAvail();
        
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - buttonSize.X / 2.0f;

        ImGui.PushItemWidth(region.X - buttonSize.X - ImGui.GetStyle().ItemSpacing.X);
        
        if (ImGui.BeginCombo("##FilterCombo", filterType?.GetLabel() ?? "Show All"))
        {
            if (ImGui.Selectable("Show All", filterType == null))
            {
                filterType = null;
            }
            
            foreach (var value in Enum.GetValues<ModuleType>())
            {
                if (ImGui.Selectable(value.GetLabel(), filterType == value))
                {
                    filterType = value;
                }
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