using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class SettingsTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {

        };

        public string TabName => Strings.Tabs.SettingsTabLabel;
        public string Description => Strings.Tabs.SettingsTabDescription;
    }
}
