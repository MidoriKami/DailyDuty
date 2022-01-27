using DailyDuty.DisplaySystem.DisplayModules;

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
                new MiniCactpot()
            };
        }
    }
}
