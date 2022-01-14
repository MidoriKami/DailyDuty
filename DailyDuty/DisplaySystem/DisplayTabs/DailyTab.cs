using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayModules;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class DailyTab : TabCategory, IDisposable
    {
        private readonly List<DisplayModule> Modules = new()
        {
            new DailyTreasureMap(),
            new WondrousTails()
        };

        public DailyTab()
        {
            CategoryName = "Daily Reminders";
            TabName = "Daily";
        }

        protected override void DrawContents()
        {
            ImGui.BeginChildFrame(1, new Vector2(490, 365), ImGuiWindowFlags.NoBackground);

            foreach (var module in Modules)
            {
                module.Draw();
            }

            ImGui.EndChildFrame();
        }

        public override void Dispose()
        {
            foreach (var module in Modules)
            {
                module.Dispose();
            }
        }
    }
}
