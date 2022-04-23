using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.ModuleConfiguration.Features;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class FeaturesTab : ITab
    {
        public IConfigurable? SelectedTabItem { get; set; }
        public List<IConfigurable> TabItems { get; set; } = new()
        {
            new WondrousTailsDutyFinderOverlay(),
            new DutyRouletteDutyFinderOverlay()
        };

        public string TabName => Strings.Tabs.FeaturesTabLabel;
        public string Description => Strings.Tabs.FeaturesTabDescription;
    }
}
