using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Daily;

internal class GrandCompany : 
    ICollapsibleHeader, 
    IUpdateable,
    ICompletable
{
    private GrandCompanySettings Settings => Service.Configuration.Current().GrandCompany;

    public CompletionType Type => CompletionType.Daily;
    public string HeaderText => "Grand Company";
    public GenericSettings GenericSettings => Settings;
    public bool IsCompleted()
    {
        return false;
    }

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