﻿using System;
using System.Collections.Generic;
using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Models.Attributes;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleDataView
{
    public static void Draw(List<(FieldInfo, DataDisplay)> fields, ModuleDataBase moduleData)
    {
        if(fields.Count > 0)
        {
            ImGui.Text("Module Data");
            ImGui.Separator();
            
            ImGuiHelpers.ScaledIndent(15.0f);

            if (ImGui.BeginTable("##DataTable", 2, ImGuiTableFlags.SizingStretchSame))
            {
                foreach (var (field, attribute) in fields)
                {
                    DrawGenericData(moduleData, field, attribute);
                }
                
                ImGui.EndTable();
            }

            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    private static void DrawGenericData(ModuleDataBase moduleData, FieldInfo field, DataDisplay attribute)
    {
        switch (Type.GetTypeCode(field.FieldType))
        {
            case TypeCode.Boolean:
                var boolValue = (bool) field.GetValue(moduleData)!;
                
                ImGui.TableNextColumn();
                ImGui.Text(attribute.Label);
                
                ImGui.TableNextColumn();
                ImGui.Text(boolValue.ToString());
                break;

            case TypeCode.String:
                var stringValue = (string) field.GetValue(moduleData)!;
                
                ImGui.TableNextColumn();
                ImGui.Text(attribute.Label);
                
                ImGui.TableNextColumn();
                ImGui.Text(stringValue);
                break;
        }
    }
}