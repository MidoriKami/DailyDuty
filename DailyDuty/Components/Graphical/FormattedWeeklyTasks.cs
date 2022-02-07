using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical;

internal class FormattedWeeklyTasks : ITaskCategoryDisplay
{
    private readonly List<ICompletable> tasks;
    List<ICompletable> ITaskCategoryDisplay.Tasks => tasks;

    public FormattedWeeklyTasks(List<ICompletable> tasks)
    {
        this.tasks = tasks;
    }

    public string HeaderText { get; } = "Weekly Tasks";
}