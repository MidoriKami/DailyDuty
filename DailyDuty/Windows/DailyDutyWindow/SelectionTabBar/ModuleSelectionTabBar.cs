using System.Collections.Generic;
using DailyDuty.Interfaces;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class ModuleSelectionTabBar : ITabBar
    {
        public string TabBarName => "ModuleSelectionTabBar";

        public List<ITab> Tabs { get; } = new()
        {
            new FeaturesTab(),
            new TimersTab(),
            new TasksTab(),
            new SettingsTab(),
            new DebugTab()
        };
    }
}
