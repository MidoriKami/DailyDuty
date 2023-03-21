using System;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public class TodoHeaderLabelEditView
{
    public static void Draw(TodoConfig config, Action saveAction)
    {
        ImGui.Text("Todo Display Enable");
        ImGui.Separator();
            
        ImGuiHelpers.ScaledIndent(15.0f);
        ImGui.InputText("Daily Label", ref config.DailyLabel, 2048);
        if(ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        
        ImGui.InputText("Weekly Label", ref config.WeeklyLabel, 2048);
        if(ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        
        ImGui.InputText("Special Label", ref config.SpecialLabel, 2048);
        if(ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
            
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}