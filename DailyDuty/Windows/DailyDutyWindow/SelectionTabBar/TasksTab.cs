﻿using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.ModuleConfiguration.Features;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class TasksTab : ITab
    {
        public IConfigurable? SelectedTabItem { get; set; }
        public List<IConfigurable> TabItems { get; set; } = new()
        {
            new DutyRoulette(),
        };

        public string TabName => Strings.Tabs.TasksTabLabel;
        public string Description => Strings.Tabs.TasksTabDescription;
    }
}