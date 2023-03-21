using System;
using System.Numerics;
using DailyDuty.System;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public unsafe class TodoPositionEditView
{
    public static void Draw(AtkTextNode* textNode, TodoConfig config, Action saveAction)
    {
        ImGui.Text("Todo Display Position");
        ImGui.Separator();
        
        ImGuiHelpers.ScaledIndent(15.0f);
        
        ImGui.Text("Control + Left Click the sliders to input exact value");

        var position = new Vector2(textNode->AtkResNode.X, textNode->AtkResNode.Y);
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.SliderFloat($"##xPos_atkUnitBase#{(ulong) textNode:X}", ref position.X, position.X - 10, position.X + 10))
        {
            textNode->AtkResNode.SetPositionFloat(position.X, position.Y);
            config.TextNodePosition = position;
        }
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            saveAction.Invoke();
        }

        ImGui.SameLine();
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.SliderFloat($"Position##yPos_atkUnitBase#{(ulong) textNode:X}", ref position.Y, position.Y - 10, position.Y + 10))
        {
            textNode->AtkResNode.SetPositionFloat(position.X, position.Y);
            config.TextNodePosition = position;
        }
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            saveAction.Invoke();
        }
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}