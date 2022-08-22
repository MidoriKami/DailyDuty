using DailyDuty.Modules.Enums;
using System;
using DailyDuty.Utilities;

namespace DailyDuty.Interfaces;

internal interface ILogicComponent
{
    IModule ParentModule { get; }

    ModuleStatus GetModuleStatus();

    string GetStatusMessage();

    void OnLoginMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnLogin.Value) return;
        if (ParentModule.LogicComponent.GetModuleStatus() != ModuleStatus.Incomplete) return;

        var moduleName = ParentModule.Name.GetLocalizedString();

        Chat.Print(moduleName, GetStatusMessage());
    }        
        
    void OnZoneChangeMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnZoneChange.Value) return;
        if (Condition.IsBoundByDuty()) return;
        if (ParentModule.LogicComponent.GetModuleStatus() != ModuleStatus.Incomplete) return;

        var moduleName = ParentModule.Name.GetLocalizedString();

        Chat.Print(moduleName, GetStatusMessage());
    }

    DateTime GetNextReset();

    void DoReset();
}