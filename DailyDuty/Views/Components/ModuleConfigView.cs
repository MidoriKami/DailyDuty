using System;
using System.Collections.Generic;
using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Action = System.Action;

namespace DailyDuty.Views.Components;

public static class ModuleConfigView
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
            }
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    private static void DrawGenericConfig(ModuleConfigBase moduleConfig, Action saveAction, FieldInfo field, ConfigOption attribute)
    {
        if (Type.GetTypeCode(field.FieldType) == TypeCode.Boolean)
        {
            var boolValue = (bool) field.GetValue(moduleConfig)!;
            if (ImGui.Checkbox(attribute.Name, ref boolValue))
            {
                field.SetValue(moduleConfig, boolValue);
                saveAction.Invoke();
            }
            if(attribute.HelpText is not null) ImGuiComponents.HelpMarker(attribute.HelpText);
            return;
        }

        if (ImGui.BeginTable($"##ValueTable{field.Name}", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            
            switch (Type.GetTypeCode(field.FieldType))
            {
                case TypeCode.String:
                    var stringValue = (string) field.GetValue(moduleConfig)!;

                    ImGui.InputText($"##{field.Name}", ref stringValue, 2048);
                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        field.SetValue(moduleConfig, stringValue);
                        saveAction.Invoke();
                    }
                    break;
                
                case TypeCode.Int32 when !field.FieldType.IsSubclassOf(typeof(Enum)):
                    var intValue = (int) field.GetValue(moduleConfig)!;

                    if(ImGui.SliderInt($"##{field.Name}", ref intValue, attribute.IntMin, attribute.IntMax))
                    {
                        field.SetValue(moduleConfig, intValue);
                        saveAction.Invoke();
                    }
                    break;
                
                default:
                    if (field.FieldType.IsSubclassOf(typeof(Enum)))
                    {
                        var enumObject = (Enum) field.GetValue(moduleConfig)!;

                        if (GenericEnumView.DrawEnumCombo(ref enumObject))
                        {
                            saveAction.Invoke();

                        }
                    }
                    break;
            }
            
            ImGui.TableNextColumn();
            ImGui.Text(attribute.Name);
            if(attribute.HelpText is not null) ImGuiComponents.HelpMarker(attribute.HelpText);

            ImGui.EndTable();
        }
    }
}