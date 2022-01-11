using System;
using DailyDuty.Reminders.Daily.DailyModules;

namespace DailyDuty.Reminders.Daily
{
    internal class DailyTab : TabCategory, IDisposable
    {
        private readonly ReminderModule dailyTreasureMap = new DailyTreasureMap();

        public DailyTab()
        {
            CategoryName = "Daily Reminders";
            TabName = "Daily";
        }

        protected override void DrawContents()
        {
            dailyTreasureMap.Draw();
        }

        public override void Dispose()
        {
            dailyTreasureMap.Dispose();
        }
    }
}
