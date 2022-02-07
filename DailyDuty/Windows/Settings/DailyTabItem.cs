using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Daily;

namespace DailyDuty.Windows.Settings;

internal class DailyTabItem : ITabItem
{
    private readonly List<object> headers = new()
    {
        new MiniCactpot(),
        new BeastTribe(),
        new DutyRoulette(),
        new GrandCompany(),
        new Levequests(),
        new TreasureMap()
    };

    public void Dispose()
    {
        foreach (var header in headers.OfType<ICollapsibleHeader>())
        {
            header.Dispose();
        }
    }

    public string TabName { get; } = "Daily";

    public void Draw()
    {
        foreach (var header in headers.OfType<ICollapsibleHeader>())
        {
            header.Draw();
        }
    }
}