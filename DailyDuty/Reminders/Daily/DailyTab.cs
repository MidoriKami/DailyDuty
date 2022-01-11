using System;
using System.Numerics;
using DailyDuty.Reminders.Daily.DailyModules;
using ImGuiNET;

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
            ImGui.BeginChildFrame(1, new Vector2(490, 365), ImGuiWindowFlags.NoBackground);

            dailyTreasureMap.Draw();

            ImGui.EndChildFrame();
        }

        public override void Dispose()
        {
            dailyTreasureMap.Dispose();
        }
    }
}
