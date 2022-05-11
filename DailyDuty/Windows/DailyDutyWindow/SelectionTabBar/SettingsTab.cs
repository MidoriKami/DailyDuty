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
            new StyleTabItem(),
            new NotificationsTabItem(),
            new LanguageSelectTabItem(),
        };

        public string TabName => $"{Strings.Tabs.SettingsTabLabel}###SettingsTab";
        public string Description => Strings.Tabs.SettingsTabDescription;
    }
}
