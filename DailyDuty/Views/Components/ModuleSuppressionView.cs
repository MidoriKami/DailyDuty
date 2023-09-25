using System;
using System.Numerics;
using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Interface.Utility;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleSuppressionView
{
    public static void Draw(IModuleConfigBase config, Action saveAction)
    {
        ImGui.Text(Strings.ModuleSuppression);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        var region = ImGui.GetContentRegionAvail();
        var text = Strings.ModuleSuppressionHelp;
        var textSize = ImGui.CalcTextSize(text);
        
        ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = region.X / 2.0f - textSize.X / 2.0f});
        ImGui.Text(text);

        var buttonSize = new Vector2(region.X - 15.0f * ImGuiHelpers.GlobalScale, 23.0f * ImGuiHelpers.GlobalScale);
        
        var hotkeyPressed = ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl;
        
        if(!hotkeyPressed) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        
        if (config.Suppressed)
        {
            if (ImGui.Button(Strings.UnSnooze, buttonSize) && hotkeyPressed)
            {
                config.Suppressed = false;
                saveAction.Invoke();
            }
        }
        else
        {
            if (ImGui.Button(Strings.Snooze, buttonSize) && hotkeyPressed)
            {
                config.Suppressed = true;
                saveAction.Invoke();
            }
        }

        if(!hotkeyPressed) ImGui.PopStyleVar();
        
        if (ImGui.IsItemHovered() && !hotkeyPressed)
        {
            ImGui.SetTooltip("Hold Shift + Control while clicking activate button");
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}