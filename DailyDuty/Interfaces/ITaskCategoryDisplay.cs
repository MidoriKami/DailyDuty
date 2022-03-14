using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ITaskCategoryDisplay
    {
        protected List<ICompletable> Tasks { get; }
        public TaskColors Colors { get; }

        public string HeaderText { get; }
        private bool ShowCompletedTasks => Service.Configuration.Windows.Todo.ShowTasksWhenComplete;
        
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

            if (allTasksComplete == true && !ShowCompletedTasks)
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