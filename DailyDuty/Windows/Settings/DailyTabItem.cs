using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.Settings;

internal class DailyTabItem : ITabItem
{
    private readonly List<ICollapsibleHeader> headers;

    public DailyTabItem()
    {
        headers = Service.ModuleManager.GetCollapsibleHeaders(CompletionType.Daily);
    }

    public void Dispose()
    {
        foreach (var header in headers.OfType<ICollapsibleHeader>())
        {
            header.Dispose();
        }
    }

    public string TabName => "Daily";

    public void Draw()
    {
        foreach (var header in headers.OfType<ICollapsibleHeader>())
        {
            header.Draw();
        }
    }
}