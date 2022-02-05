using DailyDuty.DisplaySystem.DisplayModules;
using DailyDuty.DisplaySystem.DisplayModules.Weekly;
using DailyDuty.DisplaySystem.Windows;

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
               new CustomDeliveries(),
               new JumboCactpot(),
               new FashionReport(),
               new EliteHunts()
            };
        }
    }
}
