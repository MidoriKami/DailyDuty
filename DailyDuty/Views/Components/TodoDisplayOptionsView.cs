using System;
using DailyDuty.System;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public unsafe class TodoDisplayOptionsView
{
    public static void Draw(AtkTextNode* textNode, TodoConfig config, Action saveAction)
    {
        ImGui.Text("Todo Display Options");
        ImGui.Separator();
        
        ImGuiHelpers.ScaledIndent(15.0f);

        if (ImGui.Checkbox("Show Daily Tasks", ref config.DailyTasks)) saveAction.Invoke();
        if (ImGui.Checkbox("Show Weekly Tasks", ref config.WeeklyTasks)) saveAction.Invoke();
        if (ImGui.Checkbox("Show Special Tasks", ref config.SpecialTasks)) saveAction.Invoke();
        if (ImGui.Checkbox("Hide During Quests", ref config.HideDuringQuests)) saveAction.Invoke();
        if (ImGui.Checkbox("Hide During Duties", ref config.HideInDuties)) saveAction.Invoke();

        var enumValue = (Enum) config.AlignmentType;
        if (GenericEnumView.DrawEnumCombo(ref enumValue))
        {
            config.AlignmentType = (AlignmentType) enumValue;
            textNode->AlignmentFontType = (byte) config.AlignmentType;
            saveAction.Invoke();
        }
        ImGui.SameLine();
        ImGui.Text("Text Orientation");
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}