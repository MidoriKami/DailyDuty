using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Data.Components
{
    internal class TaskCategoryDisplay
    {
        public List<ICompletable> Tasks { get; init; }
        public TaskColors Colors { get; init; }
        public string HeaderText { get; init; }
        private bool ShowCompletedTasks => Service.SystemConfiguration.Windows.Todo.ShowTasksWhenComplete;
        
        public void Draw()
        {
            ImGui.TextColored(Colors.HeaderColor, HeaderText);

            ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

            foreach (var module in Tasks)
            {
                module.DrawTask(Colors, ShowCompletedTasks);
            }

            bool allTasksComplete = Tasks
                .Where(task => task.GenericSettings.Enabled)
                .All(task => task.IsCompleted());

            if (allTasksComplete && !ShowCompletedTasks)
            {
                ImGui.TextColored(Colors.CompleteColor, "All Tasks Complete");
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
    }
}