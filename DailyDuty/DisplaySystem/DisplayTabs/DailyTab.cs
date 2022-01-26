using CheapLoc;
using DailyDuty.DisplaySystem.DisplayModules;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class DailyTab : TabCategory
    {
        public DailyTab()
        {
            CategoryName = Loc.Localize("DailyReminders", "Daily Reminders");
            TabName = Loc.Localize("Daily", "Daily");

            FrameID = (uint)DisplayManager.Tab.Daily;

            Modules = new()
            {
                new TreasureMap(),
                new MiniCactpot()
            };
        }
    }
}
