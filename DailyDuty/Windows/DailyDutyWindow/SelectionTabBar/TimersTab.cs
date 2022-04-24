using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Timers;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class TimersTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new TimersTabItem(),
        };

        public string TabName => Strings.Tabs.TimersTabLabel;
        public string Description => Strings.Tabs.TimersTabDescription;

    }
}
