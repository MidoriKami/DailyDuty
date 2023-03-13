using System;
using System.Numerics;
using DailyDuty.Abstracts;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleSuppressionView
{
    public static void Draw(ModuleConfigBase config, Action saveAction)
    {
        ImGui.Text("Module Suppression");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        
        ImGui.Text("Silence notifications until the next module reset");

        var region = ImGui.GetContentRegionAvail();
        var buttonSize = new Vector2(region.X - 15.0f * ImGuiHelpers.GlobalScale, 23.0f * ImGuiHelpers.GlobalScale);
        
        var hotkeyPressed = (ImGui.GetIO().KeyShift && ImGui.GetIO().KeyCtrl);
        
        if(!hotkeyPressed) ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
        
        if (config.Suppressed)
        {
            if (ImGui.Button("UnSnooze", buttonSize) && hotkeyPressed)
            {
                config.Suppressed = false;
                saveAction.Invoke();
            }
        }
        else
        {
            if (ImGui.Button("Snooze", buttonSize) && hotkeyPressed)
            {
                config.Suppressed = true;
                saveAction.Invoke();
            }
        }
        
        if(!hotkeyPressed) ImGui.PopStyleVar();
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}