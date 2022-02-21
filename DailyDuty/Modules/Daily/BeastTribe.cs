using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Modules.Daily;

internal class BeastTribe : 
    ICollapsibleHeader, 
    IUpdateable,
    ICompletable
{
    private BeastTribeSettings Settings => Service.Configuration.Current().BeastTribe;
    public CompletionType Type => CompletionType.Daily;
    public string HeaderText => "Beast Tribe";
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