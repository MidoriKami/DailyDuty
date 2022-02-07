using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Weekly;

namespace DailyDuty.Windows.Settings;

internal class WeeklyTabItem : ITabItem
{
    private readonly List<ICollapsibleHeader> headers = new()
    {
        new BlueMageLog(),
        new ChallengeLog(),
        new CustomDelivery(),
        new DomanEnclave(),
        new FashionReport(),
        new HuntMarks(),
        new JumboCactpot(),
        new MaskedCarnival(),
        new WondrousTails()
    };

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