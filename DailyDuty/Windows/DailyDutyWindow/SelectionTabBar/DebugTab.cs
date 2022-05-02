using System.Collections.Generic;
using DailyDuty.Graphical.TabItems;
using DailyDuty.Interfaces;
using DailyDuty.Localization;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class DebugTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }

        public List<ITabItem> TabItems { get; set; } = new()
        {
            new LogDebugTabItem(),
        };

        public string TabName => Strings.Tabs.DebugTabLabel;
        public string Description => Strings.Tabs.DebugTabDescription;

    }
}
