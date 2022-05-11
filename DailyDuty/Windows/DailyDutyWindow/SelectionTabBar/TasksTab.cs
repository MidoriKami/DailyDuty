using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.ModuleConfiguration;
using DailyDuty.Timers;

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
            new FashionReport(),
            new JumboCactpot(),
            new HuntMarksWeekly(),
            new HuntMarksDaily(),
            new TimersTabItem(),
        };

        public string TabName => $"{Strings.Tabs.TasksTabLabel}###TasksTab";
        public string Description => Strings.Tabs.TasksTabDescription;
    }
}
