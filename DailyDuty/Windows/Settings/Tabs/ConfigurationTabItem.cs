using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Settings.Headers;

namespace DailyDuty.Windows.Settings.Tabs
{
    internal class ConfigurationTabItem : ITabItem
    {
        private readonly List<ICollapsibleHeader> headers = new()
        {
            new GeneralConfiguration(),
            new CountdownTimersConfiguration(),
            new DutyFinderOverlayConfiguration(),
            new WindowConfiguration()
        };

        public static bool EditModeEnabled = false;

        public string TabName => "Configuration";

        public void Draw()
        {
            Utilities.Draw.Checkbox("Temporary Edit Mode", TabName, ref ConfigurationTabItem.EditModeEnabled, 
                "Allows you to manually correct the values stored in each of Daily/Weekly tabs\n" +
                "Edit Mode automatically disables when you close this window\n" +
                "Only use Edit Mode to correct errors in other tabs");

            foreach (var header in headers)
            {
                header.Draw();
            }
        }

        public void Dispose()
        {
        }
    }
}