using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Interfaces;

namespace DailyDuty.Components.Graphical;

internal class FormattedDailyTasks : ITaskCategoryDisplay
{
    private readonly List<ICompletable> tasks;
    List<ICompletable> ITaskCategoryDisplay.Tasks => tasks;
    public Vector4 HeaderColor => Service.Configuration.TodoWindowSettings.HeaderColor;

    public Vector4 ItemIncompleteColor => Service.Configuration.TodoWindowSettings.IncompleteColor;

    public Vector4 ItemCompleteColor => Service.Configuration.TodoWindowSettings.CompleteColor;

    public FormattedDailyTasks(List<ICompletable> tasks)
    {
        this.tasks = tasks;
    }

    public string HeaderText => "Daily Tasks";
}