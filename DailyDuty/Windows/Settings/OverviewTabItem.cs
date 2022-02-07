using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Components.Graphical;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings;

internal class OverviewTabItem : ITabItem
{
    public string TabName => "Overview";

    private readonly ITaskCategoryDisplay dailyTasks;
    private readonly ITaskCategoryDisplay weeklyTasks;
    public OverviewTabItem()
    {
        dailyTasks = Service.ModuleManager.GetDailyTasks();
        weeklyTasks = Service.ModuleManager.GetWeeklyTasks();
    }

    public void Draw()
    {
        dailyTasks.Draw();

        weeklyTasks.Draw();
    }

    public void Dispose()
    {
    }
}