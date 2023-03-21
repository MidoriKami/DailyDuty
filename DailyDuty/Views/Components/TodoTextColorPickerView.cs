using System.Numerics;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;
using Action = System.Action;

namespace DailyDuty.Views.Components;

public class TodoTextColorPickerView
{
    public static void Draw(TodoConfig config, Action saveAction)
    {
        ImGui.Text("Todo Display Text Style");
        ImGui.Separator();
        
        ImGuiHelpers.ScaledIndent(15.0f);

        ImGui.ColorEdit4("##TextColor", ref config.TextColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf);
        if (ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        ImGui.SameLine();
        if (ImGui.Button("Reset##TextColor"))
        {
            config.TextColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            saveAction.Invoke();
        }
        ImGui.SameLine();
        ImGui.Text("Text Color");

        ImGui.ColorEdit4("##TextBorder", ref config.TextBorderColor, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf);
        if (ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        ImGui.SameLine();
        if (ImGui.Button("Reset##BorderColor"))
        {
            config.TextBorderColor = new Vector4(142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f);
            saveAction.Invoke();
        }
        ImGui.SameLine();
        ImGui.Text("Text Border Color");
        
        UiColorPickerView.SelectorButton(ref config.HeaderGlowKey);
        ImGui.SameLine();
        if (ImGui.Button("Reset##UiGlow"))
        {
            config.HeaderGlowKey = 14;
            saveAction.Invoke();
        }
        ImGui.SameLine();
        ImGui.Text("Header Outline Color");

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}