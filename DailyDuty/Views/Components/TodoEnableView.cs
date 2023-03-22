using System;
using DailyDuty.System;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public class TodoEnableView
{
    public static void Draw(TodoConfig config, Action saveAction)
    {
        ImGui.Text(Strings.TodoDisplayEnable);
        ImGui.Separator();
            
        ImGuiHelpers.ScaledIndent(15.0f);
        
        if (ImGui.Checkbox($"{Strings.Enable}##TodoEnable", ref config.Enable))
        {
            saveAction.Invoke();
        }
            
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}