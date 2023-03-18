using System;
using DailyDuty.Models.Attributes;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class GenericEnumView
{
    public static bool DrawEnumCombo(ref Enum value)
    {
        var valueChanged = false;
        
        if (ImGui.BeginCombo($"##EnumCombo{value.GetType()}", value.GetLabel()))
        {
            foreach (Enum enumValue in Enum.GetValues(value.GetType()))
            {
                if (ImGui.Selectable(enumValue.GetLabel(), enumValue.Equals(value)))
                {
                    value = enumValue;
                    valueChanged = true;
                }
            }
            
            ImGui.EndCombo();
        }

        return valueChanged;
    }
}