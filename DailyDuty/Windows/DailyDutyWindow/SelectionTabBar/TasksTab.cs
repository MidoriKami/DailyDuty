using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.ModuleConfiguration;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class TasksTab : ITab
    {
        public ITabItem? SelectedTabItem { get; set; }
        public List<ITabItem> TabItems { get; set; } = new()
        {
            new DutyRoulette(),
            new WondrousTails(),
            new TreasureMap(),
            new BeastTribe(),
            new Levequest(),
            new MiniCactpot(),
            new CustomDelivery(),
            new DomanEnclave(),
        };

        public string TabName => Strings.Tabs.TasksTabLabel;
        public string Description => Strings.Tabs.TasksTabDescription;
    }
}
