using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Configuration.Components;
using Dalamud.Interface;
using Dalamud.Interface.Components;
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

    public static Action GetConfigCheckboxAction(string label, Setting<bool> setting, string? helpText = null)
    {
        return () =>
        {
            if (ImGui.Checkbox("", ref setting.Value))
            {
                Service.ConfigurationManager.Save();
            }

            ImGui.SameLine(27.0f * ImGuiHelpers.GlobalScale);

            ImGui.TextUnformatted(label);

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }
        };
    }

    public static Action GetConfigComboAction<T>(IEnumerable<T> values, Setting<T> setting, Func<T, string> localizeEnum, string label = "", float width = 0.0f) where T : struct
    {
        return () =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            if (ImGui.BeginCombo(label, localizeEnum(setting.Value)))
            {
                foreach (var value in values)
                {
                    if (ImGui.Selectable(localizeEnum(value), setting.Value.Equals(value)))
                    {
                        setting.Value = value;
                        Service.ConfigurationManager.Save();
                    }
                }

                ImGui.EndCombo();
            }
        };
    }

    public static Action GetSliderInt(string label, Setting<int> setting, int minValue, int maxValue, float width = 0.0f)
    {
        return () =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.SliderInt(label, ref setting.Value, minValue, maxValue);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.ConfigurationManager.Save();
            }
        };
    }

    public static Action GetDragFloat(string label, Setting<float> setting, float minValue, float maxValue, float width = 0.0f)
    {
        return () =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.DragFloat(label, ref setting.Value, 0.01f, minValue, maxValue, "%.2f");
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.ConfigurationManager.Save();
            }
        };
    }

    public static Action GetConfigColor(string label, Setting<Vector4> color)
    {
        return () =>
        {
            if (ImGui.ColorEdit4(label, ref color.Value, ImGuiColorEditFlags.NoInputs))
            {
                Service.ConfigurationManager.Save();
            }
        };
    }

    public static Action GetConfigRadio<T>(string label, Setting<T> setting, T buttonValue, string? helpText = null) where T : struct
    {
        return () =>
        {
            var value = Convert.ToInt32(setting.Value);

            if (ImGui.RadioButton(label, ref value, Convert.ToInt32(buttonValue)))
            {
                setting.Value =  (T) Enum.ToObject(typeof(T), value);
                Service.ConfigurationManager.Save();
            }

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }
        };
    }

    public static Action GetConfigString(Setting<string> settingsCustomName, float width = 0.0f)
    {
        return () =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }

            ImGui.InputText("", ref settingsCustomName.Value, 24);

            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.ConfigurationManager.Save();
            }
        };
    }
}