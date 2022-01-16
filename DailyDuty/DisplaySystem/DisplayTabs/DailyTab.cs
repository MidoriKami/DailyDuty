using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayModules;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class DailyTab : TabCategory
    {
        public DailyTab()
        {
            CategoryName = "Daily Reminders";
            TabName = "Daily";

            FrameID = (uint)DisplayManager.Tab.Daily;

            Modules = new()
            {
                new DailyTreasureMap()
            };
        }
    }
}
