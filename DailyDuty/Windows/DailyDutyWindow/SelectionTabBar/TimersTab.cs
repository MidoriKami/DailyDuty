using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class TimersTab : ITab
    {
        public IConfigurable? SelectedTabItem { get; set; }
        public List<IConfigurable> TabItems { get; set; } = new()
        {

        };

        public string TabName => Strings.Tabs.TimersTabLabel;
        public string Description => Strings.Tabs.TimersTabDescription;
    }
}
