using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly;

internal class BlueMageLog : ICollapsibleHeader, IUpdateable
{
    public void Dispose()
    {

    }

    public string HeaderText { get; } = "Blue Mage Log";
    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Text("Not Implemented Yet");
    }

    public void Update()
    {
            
    }

}