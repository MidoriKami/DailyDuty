using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly;

internal class MaskedCarnival : 
    ICollapsibleHeader, 
    IUpdateable,
    ICompletable
{
    public void Dispose()
    {
    }

    private MaskedCarnivalSettings Settings => Service.Configuration.Current().MaskedCarnival;

    public CompletionType Type => CompletionType.Weekly;
    public string HeaderText => "Masked Carnival";
    public GenericSettings GenericSettings => Settings;
    public bool IsCompleted()
    {
        return false;
    }

    void ICollapsibleHeader.DrawContents()
    {
        ImGui.Text("Not Implemented Yet");
    }

    public void Update()
    {
            
    }
}