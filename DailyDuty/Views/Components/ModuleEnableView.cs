using System;
using DailyDuty.Abstracts;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleEnableView
{
    public static void Draw(ModuleConfigBase config, Action saveAction)
    {
        ImGui.Text("Module Enable");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        if (ImGui.Checkbox($"Enable##ModuleEnable", ref config.ModuleEnabled))
        {
            saveAction.Invoke();
        }
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}