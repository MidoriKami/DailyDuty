using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using Dalamud.Logging;
using ImGuiNET;

namespace DailyDuty.Modules.Daily;

internal class BeastTribe : ICollapsibleHeader, IUpdateable
{
    private BeastTribeSettings Settings => Service.Configuration.Current().BeastTribe;
    public string HeaderText => "Beast Tribe";

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