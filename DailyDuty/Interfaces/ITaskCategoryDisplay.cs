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

    public Vector4 HeaderColor { get; set; }
    public Vector4 ItemIncompleteColor { get; set;}
    public Vector4 ItemCompleteColor { get; set; }

    public string HeaderText { get; }
        
    public void Draw()
    {
        ImGui.TextColored(HeaderColor, HeaderText);
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
            ImGui.TextColored(ItemCompleteColor, "All Tasks Complete");
            ImGui.Spacing();
        }

        ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
    }

    public bool AllTasksCompleted()
    {
        return Tasks
            .Where(task => task.GenericSettings.Enabled)
            .All(task => task.IsCompleted());
    }

    private void DrawTaskStatus(ICompletable task)
    {
        if (task.IsCompleted() == false && task.GenericSettings.Enabled)
        {
            ImGui.TextColored(ItemIncompleteColor, task.HeaderText);
            ImGui.Spacing();
        }
    }
}