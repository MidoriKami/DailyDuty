using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using DailyDuty.Models.Attributes;
using DailyDuty.System;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Action = System.Action;

namespace DailyDuty.Views.Components;

public static class GenericConfigView
{
    public static void Draw(List<(FieldInfo, ConfigOption)> fields, object sourceObject, Action saveAction, string sectionName)
    {
        if(fields.Count > 0)
        {
            ImGui.Text(sectionName);
            ImGui.Separator();
            
            ImGuiHelpers.ScaledIndent(15.0f);
        
            foreach (var (field, attribute) in fields)
            {
                DrawGenericConfig(sourceObject, saveAction, field, attribute);
            }
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    private static void DrawGenericConfig(object sourceObject, Action saveAction, FieldInfo field, ConfigOption attribute)
    {
        if (Type.GetTypeCode(field.FieldType) == TypeCode.Boolean)
        {
            var boolValue = (bool) field.GetValue(sourceObject)!;
            if (ImGui.Checkbox(attribute.Name, ref boolValue))
            {
                field.SetValue(sourceObject, boolValue);
                saveAction.Invoke();
            }
            if(attribute.HelpText is not null) ImGuiComponents.HelpMarker(attribute.HelpText);
            return;
        }
        else if (field.FieldType == typeof(Vector4))
        {
            var vectorValue = (Vector4) field.GetValue(sourceObject)!;
            if (ImGui.ColorEdit4($"##{field.Name}", ref vectorValue, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf))
            {
                field.SetValue(sourceObject, vectorValue);
                saveAction.Invoke();
            }
            ImGui.SameLine();
            if (ImGui.Button($"{Strings.Default}##{field.Name}"))
            {
                field.SetValue(sourceObject, attribute.DefaultColor);
                saveAction.Invoke();
            }
            ImGui.SameLine();
            ImGui.Text(attribute.Name);
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
                    var stringValue = (string) field.GetValue(sourceObject)!;

                    if (attribute.UseAxisFont) ImGui.PushFont(DailyDutyPlugin.System.FontController.Axis12.ImFont);
                    if (ImGui.InputTextWithHint($"##{field.Name}", attribute.Name, ref stringValue, 2048))
                    {
                        field.SetValue(sourceObject, stringValue);
                    }
                    if (attribute.UseAxisFont) ImGui.PopFont();

                    if (ImGui.IsItemDeactivatedAfterEdit())
                    {
                        saveAction.Invoke();
                    }
                    break;

                case TypeCode.Int32 when !field.FieldType.IsSubclassOf(typeof(Enum)):
                    var intValue = (int) field.GetValue(sourceObject)!;

                    if (ImGui.SliderInt($"##{field.Name}", ref intValue, attribute.IntMin, attribute.IntMax))
                    {
                        field.SetValue(sourceObject, intValue);
                        saveAction.Invoke();
                    }
                    break;

                case TypeCode.Single:
                    var floatValue = (float) field.GetValue(sourceObject)!;

                    if (ImGui.SliderFloat($"##{field.Name}", ref floatValue, attribute.FloatMin, attribute.FloatMax))
                    {
                        field.SetValue(sourceObject, floatValue);
                        saveAction.Invoke();
                    }
                    break;

                default:
                    if (field.FieldType.IsSubclassOf(typeof(Enum)))
                    {
                        var enumObject = (Enum) field.GetValue(sourceObject)!;

                        if (GenericEnumView.DrawEnumCombo(ref enumObject))
                        {
                            field.SetValue(sourceObject, enumObject);
                            saveAction.Invoke();
                        }
                    }
                    else if (field.FieldType == typeof(Vector2))
                    {
                        var vectorValue = (Vector2) field.GetValue(sourceObject)!;
                        if (ImGui.DragFloat2($"##{field.Name}", ref vectorValue, 5.0f))
                        {
                            field.SetValue(sourceObject, vectorValue);
                            saveAction.Invoke();
                        }
                    }
                    break;
            }
            
            ImGui.TableNextColumn();
            if(attribute.ShowLabel) ImGui.Text(attribute.Name);
            if(attribute.HelpText is not null) ImGuiComponents.HelpMarker(attribute.HelpText);

            ImGui.EndTable();
        }
    }
}