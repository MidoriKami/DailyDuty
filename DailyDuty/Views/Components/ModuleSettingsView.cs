using System;
using System.Collections.Generic;
using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleSettingsView
{
    public static void Draw(List<(FieldInfo, ConfigOption)> fields, ModuleConfigBase moduleConfig, Action saveAction)
    {
        if(fields.Count > 0)
        {
            ImGui.Text("Module Options");
            ImGui.Separator();
            
            ImGuiHelpers.ScaledIndent(15.0f);
        
            foreach (var (field, attribute) in fields)
            {
                DrawGenericConfig(moduleConfig, saveAction, field, attribute);

                if (attribute.HelpText is not null) ImGuiComponents.HelpMarker(attribute.HelpText);
            }
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    private static void DrawGenericConfig(ModuleConfigBase moduleConfig, Action saveAction, FieldInfo field, ConfigOption attribute)
    {
        switch (Type.GetTypeCode(field.FieldType))
        {
            case TypeCode.Boolean:
                var boolValue = (bool) field.GetValue(moduleConfig)!;
                if (ImGui.Checkbox(attribute.Name, ref boolValue))
                {
                    field.SetValue(moduleConfig, boolValue);
                    saveAction.Invoke();
                }
                break;

            case TypeCode.String:
                var stringValue = (string) field.GetValue(moduleConfig)!;
                ImGui.InputText(attribute.Name + $"##{field.Name}", ref stringValue, 2048);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    field.SetValue(moduleConfig, stringValue);
                    saveAction.Invoke();
                }
                break;
        }
    }
}