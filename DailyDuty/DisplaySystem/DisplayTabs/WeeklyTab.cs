using CheapLoc;
using DailyDuty.DisplaySystem.DisplayModules;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class WeeklyTab : TabCategory
    {
        public WeeklyTab()
        {
            CategoryName = Loc.Localize("Weekly Reminders", "Weekly Reminders");
            TabName = Loc.Localize("Weekly Reminders", "Weekly Reminders");

            FrameID = (uint) DisplayManager.Tab.Weekly;

            Modules = new()
            {
               new WondrousTails(),
               new CustomDeliveries()
            };
        }
    }
}
