using System.Collections.Generic;
using DailyDuty.Graphical.TabItems;
using DailyDuty.Interfaces;
using DailyDuty.Localization;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class SettingsTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new MainWindowSettingsTabItem(),
            new NotificationSettingsTabItem(),
        };

        public string TabName => Strings.Tabs.SettingsTabLabel;
        public string Description => Strings.Tabs.SettingsTabDescription;
    }
}
