using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Settings.SettingsHeaders;

namespace DailyDuty.Windows.Settings;

internal class ConfigurationTabItem : ITabItem
{
    private readonly List<ICollapsibleHeader> headers = new()
    {
        new CountdownTimersConfiguration(),
        new GeneralConfiguration(),
        new DutyFinderOverlayConfiguration(),
        new TodoWindowConfiguration(),
        new TimersWindowConfiguration()
    };

    public static bool EditModeEnabled = false;

    public string TabName => "Configuration";

    public void Draw()
    {
        foreach (var header in headers)
        {
            header.Draw();
        }
    }

    public void Dispose()
    {
    }
}