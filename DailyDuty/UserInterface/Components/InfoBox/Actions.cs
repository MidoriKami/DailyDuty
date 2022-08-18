using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Configuration.Common;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components.InfoBox;

internal static class Actions
{
    public static Action GetStringAction(string message, Vector4? color = null)
    {
        if (color == null)
        {
            return () =>
            {
                ImGui.Text(message);
            };
        }
        else
        {
            return () =>
            {
                ImGui.TextColored(color.Value, message);
            };
        }
    }

    public static Action GetConfigCheckboxAction(string label, Setting<bool> setting)
    {
        return () =>
        {
            if (ImGui.Checkbox(label, ref setting.Value))
            {
                Service.ConfigurationManager.SaveAll();
            }
        };
    }

    public static Action GetConfigComboAction<T>(IEnumerable<T> values, Setting<T> setting, string label = "", float width = 200.0f) where T : struct
    {
        return () =>
        {
            ImGui.SetNextItemWidth(width);
            if (ImGui.BeginCombo(label, setting.Value.ToString() ?? "Null Value"))
            {
                foreach (var value in values)
                {
                    if (ImGui.Selectable(value.ToString(), setting.Value.Equals(value)))
                    {
                        setting.Value = value;
                        Service.ConfigurationManager.SaveAll();
                    }
                }

                ImGui.EndCombo();
            }
        };
    }
}