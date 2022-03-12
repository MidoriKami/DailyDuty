using System.Collections.Generic;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using DailyDuty.Interfaces;

namespace DailyDuty.Components.Graphical
{
    internal class FormattedDailyTasks : ITaskCategoryDisplay
    {
        private readonly List<ICompletable> tasks;
        List<ICompletable> ITaskCategoryDisplay.Tasks => tasks;

        public TaskColors Colors => Service.Configuration.Windows.Todo.Colors;
        
        public FormattedDailyTasks(List<ICompletable> tasks)
        {
            this.tasks = tasks;
        }

        public string HeaderText => "Daily Tasks";
    }
}