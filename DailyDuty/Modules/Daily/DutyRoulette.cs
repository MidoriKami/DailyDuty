using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Daily
{
    internal class DutyRoulette : ICollapsibleHeader, IUpdateable
    {
        public string HeaderText { get; } = "Duty Roulette";
        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Text("Not Implemented Yet");
        }

        public void Dispose()
        {
        }

        public void Update()
        {
            
        }
    }
}
