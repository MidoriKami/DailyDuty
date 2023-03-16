using System;
using System.Collections.Generic;
using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleClickableLinkConfigView
{
    public static void Draw(List<(FieldInfo, ClickableLink)> fields, ModuleConfigBase moduleConfig, Action saveAction)
    {
        if (fields.Count > 0)
        {
            ImGui.Text("Clickable Link");
            ImGui.Separator();
            
            ImGuiHelpers.ScaledIndent(15.0f);

            foreach (var field in fields)
            {
                ImGui.Text(field.Item2.Description);

                var boolValue = (bool) field.Item1.GetValue(moduleConfig)!;
                if (ImGui.Checkbox($"Enable##{field.Item1.Name}", ref boolValue))
                {
                    field.Item1.SetValue(moduleConfig, boolValue);
                    saveAction.Invoke();
                }
            }
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
}