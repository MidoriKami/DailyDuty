using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Interfaces;

internal interface ITaskCategoryDisplay
{
    protected List<ICompletable> Tasks { get; }

    public string HeaderText { get; }
        
    public void Draw()
    {
        ImGui.Text(HeaderText);
        ImGui.Spacing();

        ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

        foreach (var module in Tasks)
        {
            DrawTaskStatus(module);
        }

        bool allTasksComplete = Tasks
            .Where(task => task.GenericSettings.Enabled)
            .All(task => task.IsCompleted());

        if (allTasksComplete == true)
        {
            ImGui.TextColored(new Vector4(0, 255, 0, 150), "All Tasks Complete");
            ImGui.Spacing();
        }

        ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
    }

    public bool AllTasksCompleted()
    {
        return Tasks.All(task => task.IsCompleted());
    }

    private static void DrawTaskStatus(ICompletable task)
    {
        if (task.IsCompleted() == false && task.GenericSettings.Enabled)
        {
            ImGui.TextColored(new Vector4(255, 0, 0, 150), task.HeaderText);
            ImGui.Spacing();
        }
    }
}