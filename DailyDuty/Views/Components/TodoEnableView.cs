using System;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public class TodoEnableView
{
    public static void Draw(TodoConfig config, Action saveAction)
    {
        ImGui.Text("Todo Display Enable");
        ImGui.Separator();
            
        ImGuiHelpers.ScaledIndent(15.0f);
        
        if (ImGui.Checkbox("Enable##TodoEnable", ref config.Enable))
        {
            saveAction.Invoke();
        }
            
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}