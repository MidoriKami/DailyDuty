using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly;

internal class JumboCactpot : ICollapsibleHeader, IUpdateable
{
    public void Dispose()
    {

    }

    public string HeaderText => "Jumbo Cactpot";

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Text("Temporarily Removed, look forward to updates");
    }

    public void Update()
    {
            
    }
}