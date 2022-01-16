using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.DisplaySystem.DisplayModules;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class WeeklyTab : TabCategory
    {
        public WeeklyTab()
        {
            CategoryName = "Weekly Reminders";
            TabName = "Weekly";

            FrameID = (uint) DisplayManager.Tab.Weekly;

            Modules = new()
            {
                new WondrousTails(),
                new CustomDeliveries()
            };
        }
    }
}
