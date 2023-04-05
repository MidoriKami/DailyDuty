using System;
using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public class ModuleTodoSettings
{
    public static void Draw(ModuleConfigBase config, Action saveAction)
    {
        ImGui.Text(Strings.TodoDisplayOptions);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        DrawCustomLabelOption(config, saveAction);

        
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
    
    private static void DrawCustomLabelOption(ModuleConfigBase config, Action saveAction)
    {
        
        if (ImGui.Checkbox("Use Custom Label", ref config.UseCustomTodoLabel))
        {
            saveAction.Invoke();
        }

        if (config.UseCustomTodoLabel)
        {
            ImGui.PushFont(DailyDutyPlugin.System.FontController.Axis12.ImFont);
            ImGui.InputTextWithHint("##CustomResetMessage", Strings.TaskLabel, ref config.CustomTodoLabel, 2048);
            ImGui.PopFont();
            if (ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        }
    }
}