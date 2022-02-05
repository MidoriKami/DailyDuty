using DailyDuty.DisplaySystem.DisplayModules;
using DailyDuty.DisplaySystem.DisplayModules.Daily;
using DailyDuty.DisplaySystem.Windows;

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
                new TreasureMap(),
                new MiniCactpot(),
                new Roulettes()
            };
        }
    }
}
