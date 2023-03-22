using System;
using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleEnableView
{
    public static void Draw(ModuleConfigBase config, Action saveAction)
    {
        ImGui.Text(Strings.ModuleEnable);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        if (ImGui.Checkbox($"{Strings.Enable}##ModuleEnable", ref config.ModuleEnabled))
        {
            saveAction.Invoke();
        }
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}