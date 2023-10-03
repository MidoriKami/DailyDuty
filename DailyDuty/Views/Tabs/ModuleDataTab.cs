using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utility;

namespace DailyDuty.Views.Tabs;

public class ModuleDataTab : ISelectionWindowTab
{
    public string TabName => Strings.ModuleData;
    public ISelectable? LastSelection { get; set; }
    
    public IEnumerable<ISelectable> GetTabSelectables()
    {
        if (DailyDutyPlugin.System.SystemConfig.HideDisabledModules)
        {
            return DailyDutySystem.ModuleController
                .GetModules()
                .Where(module => module.ModuleConfig.ModuleEnabled)
                .Select(module => new DataSelectable(module))
                .OrderBy(module => module.Module.ModuleName.Label());
        }
        
        return DailyDutySystem.ModuleController
            .GetModules()
            .Select(module => new DataSelectable(module))
            .OrderBy(module => module.Module.ModuleName.Label());
    }

    public void DrawTabExtras()
    {
        var region = ImGui.GetContentRegionAvail();

        var label = DailyDutyPlugin.System.SystemConfig.HideDisabledModules ? Strings.ShowDisabled : Strings.HideDisabled;

        if (ImGui.Button(label, region with { Y = 23.0f * ImGuiHelpers.GlobalScale }))
        {
            DailyDutyPlugin.System.SystemConfig.HideDisabledModules = !DailyDutyPlugin.System.SystemConfig.HideDisabledModules;
            DailyDutyPlugin.System.SaveSystemConfig();
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
            var currentPosition = ImGui.GetCursorPos() + new Vector2(0.0f, - itemSpacing.Y + 1.0f);
            ImGui.SetCursorPos(currentPosition);ImGui.Text(Module.ModuleName.Label());

            ImGui.TableNextColumn();
            currentPosition = ImGui.GetCursorPos() + new Vector2(0.0f, - itemSpacing.Y + 1.0f);
            ImGui.SetCursorPos(currentPosition);
            var region = ImGui.GetContentRegionAvail();

            var color = Module.ModuleStatus.Color();
            var text = Module.ModuleStatus.Label();

            // Override Status if Module is Disabled
            if (!Module.ModuleConfig.ModuleEnabled)
            {
                text = Strings.Disabled;
                color = KnownColor.Gray.Vector();
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

    private float GetLongestModuleStatusLength()
    {
        var longestStatus = Enum.GetValues<ModuleStatus>().Select(value => ImGui.CalcTextSize(value.Label())).Select(size => size.X).Prepend(0.0f).Max();
        
        var enabledLength = ImGui.CalcTextSize(Strings.Enabled);
        var disabledLength = ImGui.CalcTextSize(Strings.Disabled);

        var longestEnabledDisabled = MathF.Max(enabledLength.X, disabledLength.X);

        return MathF.Max(longestStatus, longestEnabledDisabled);
    }
}