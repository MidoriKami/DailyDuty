using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Daily;

internal class DutyRoulette : ICollapsibleHeader, IUpdateable
{
    public string HeaderText => "Duty Roulette";

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Text("Temporarily Removed, look forward to updates");
    }

    public void Dispose()
    {
    }

    public void Update()
    {
            
    }
}