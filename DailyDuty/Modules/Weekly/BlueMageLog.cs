using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.WeeklySettings;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Weekly;

internal class BlueMageLog : 
    ICollapsibleHeader, 
    IUpdateable,
    ICompletable
{
    public void Dispose()
    {

    }

    private BlueMageLogSettings Settings => Service.Configuration.Current().BlueMageLog;

    public CompletionType Type => CompletionType.Weekly;
    public string HeaderText => "Blue Mage Log";
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