﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;
using KamiLib.Interfaces;

namespace DailyDuty.Views.Tabs;

public class ModuleDataTab : ISelectionWindowTab
{
    public string TabName => Strings.ModuleData;
    public ISelectable? LastSelection { get; set; }
    public bool HideDisabledModulesInSelectWindow;
    
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        if (HideDisabledModulesInSelectWindow)
        {
            return DailyDutyPlugin.System.ModuleController
                .GetModules()
                .Where(module => module.ModuleConfig.ModuleEnabled)
                .Select(module => new DataSelectable(module))
                .OrderBy(module => module.Module.ModuleName.GetLabel());
        }
        
        return DailyDutyPlugin.System.ModuleController
            .GetModules()
            .Select(module => new DataSelectable(module))
            .OrderBy(module => module.Module.ModuleName.GetLabel());
    }

    public void DrawTabExtras()
    {
        var region = ImGui.GetContentRegionAvail();

        var label = HideDisabledModulesInSelectWindow ? Strings.ShowDisabled : Strings.HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            HideDisabledModulesInSelectWindow = !HideDisabledModulesInSelectWindow;
        }
    }
}

public class DataSelectable : ISelectable, IDrawable
{
    public BaseModule Module;
    public IDrawable Contents => this;
    public string ID => Module.ModuleName.ToString();

    public DataSelectable(BaseModule module)
    {
        Module = module;
    }
    
    public void DrawLabel()
    {
        var labelRegion = ImGui.GetContentRegionAvail();
        var genericTextSize = ImGui.CalcTextSize("SizingText");
        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, Vector2.Zero);
        
        if (ImGui.BeginTable($"##ModuleNameTable{Module.ModuleName}", 2, ImGuiTableFlags.None, labelRegion with { Y = genericTextSize.Y + itemSpacing.Y } ))
        {
            ImGui.TableSetupColumn("##ModuleName", ImGuiTableColumnFlags.WidthStretch, 4);
            ImGui.TableSetupColumn("##ModuleStatus", ImGuiTableColumnFlags.WidthFixed, GetLongestModuleStatusLength());

            ImGui.TableNextColumn();
            ImGui.Text(Module.ModuleName.GetLabel());

            ImGui.TableNextColumn();
            var region = ImGui.GetContentRegionAvail();

            var color = Module.ModuleStatus.GetColor();
            var text = Module.ModuleStatus.GetLabel();

            // Override Status if Module is Disabled
            if (!Module.ModuleConfig.ModuleEnabled)
            {
                text = Strings.Disabled;
                color = KnownColor.Gray.AsVector4();
            }

            var textSize = ImGui.CalcTextSize(text);
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + region.X - textSize.X);
            ImGui.TextColored(color, text);
            
            ImGui.EndTable();
        }
        
        ImGui.PopStyleVar();
    }
    
    public void Draw()
    {
        Module.DrawData();
    }

    private float GetLongestModuleStatusLength() => Enum.GetValues<ModuleStatus>().Select(value => ImGui.CalcTextSize(value.GetLabel())).Select(size => size.X).Prepend(0.0f).Max();
}