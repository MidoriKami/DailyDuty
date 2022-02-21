using System.Collections.Generic;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings;

internal class WeeklyTabItem : ITabItem
{
    private readonly List<ICollapsibleHeader> headers;

    public WeeklyTabItem()
    {
        headers = Service.ModuleManager.GetCollapsibleHeaders(CompletionType.Weekly);
    }

    public void Dispose()
    {
        foreach (var header in headers)
        {
            header.Dispose();
        }
    }

    public string TabName => "Weekly";

    public void Draw()
    {
        foreach (var header in headers)
        {
            header.Draw();
        }
    }
}