using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using DailyDuty.Interfaces;

namespace DailyDuty.Components.Graphical
{
    internal class FormattedWeeklyTasks : ITaskCategoryDisplay
    {
        private readonly List<ICompletable> tasks;
        List<ICompletable> ITaskCategoryDisplay.Tasks => tasks;
        public TaskColors Colors => Service.Configuration.Windows.Todo.Colors;
        
        public FormattedWeeklyTasks(List<ICompletable> tasks)
        {
            this.tasks = tasks;
        }

        public string HeaderText => "Weekly Tasks";
    }
}