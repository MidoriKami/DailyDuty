using System;
using System.Collections.Generic;
using System.Globalization;
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
        ImGui.TableNextColumn();
        ImGui.Text(attribute.Label);

        ImGui.TableNextColumn();
        switch (Type.GetTypeCode(field.FieldType))
        {
            case TypeCode.Boolean:
                var boolValue = (bool) field.GetValue(moduleData)!;
                
                ImGui.Text(boolValue.ToString());
                break;

            case TypeCode.String:
                var stringValue = (string) field.GetValue(moduleData)!;

                ImGui.Text(stringValue);
                break;
            
            case TypeCode.Int32:
                var intValue = (int) field.GetValue(moduleData)!;
                
                ImGui.Text(intValue.ToString());
                break;
            
            case TypeCode.UInt32:
                var uIntValue = (uint) field.GetValue(moduleData)!;
                
                ImGui.Text(uIntValue.ToString());
                break;
            
            case TypeCode.DateTime:
                var dateTime = (DateTime) field.GetValue(moduleData)!;
                    
                ImGui.Text(dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture));
                break;
            
            case TypeCode.Object:
                if (field.FieldType == typeof(List<int>))
                {
                    var list = (List<int>) field.GetValue(moduleData)!;

                    foreach (var value in list)
                    {
                        ImGui.Text(value.ToString());
                    }
                }
                else if (field.FieldType == typeof(TimeSpan))
                {
                    var value = (TimeSpan) field.GetValue(moduleData)!;

                    if (value > TimeSpan.MinValue)
                    {
                        ImGui.Text($"{value.Days:0}.{value.Hours:00}:{value.Minutes:00}:{value.Seconds:00}");
                    }
                    else
                    {
                        ImGui.Text("Time Not Available");
                    }
                }
                break;

            default:
                ImGui.Text("Error: Unable to Read Type");
                break;
        }
    }
}