using System.Collections.Generic;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using DailyDuty.Localization;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class FeaturesTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new WondrousTailsDutyFinderOverlay(),
            new DutyRouletteDutyFinderOverlay(),
            new TodoWindowConfiguration(),
            new TimersWindowConfiguration(),
        };

        public string TabName => $"{Strings.Tabs.FeaturesTabLabel}###FeaturesTab";
        public string Description => Strings.Tabs.FeaturesTabDescription;
    }
}
