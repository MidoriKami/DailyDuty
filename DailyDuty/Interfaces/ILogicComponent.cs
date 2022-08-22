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
        var moduleEnabled = ParentModule.GenericSettings.Enabled.Value;
        var loginMessageEnabled = ParentModule.GenericSettings.NotifyOnLogin.Value;
        var moduleStatus = ParentModule.LogicComponent.GetModuleStatus();

        if (moduleEnabled && loginMessageEnabled && moduleStatus == ModuleStatus.Incomplete)
        {
            var moduleName = ParentModule.Name.GetLocalizedString();

            Chat.Print(moduleName, GetStatusMessage());
        }
    }        
        
    void OnZoneChangeMessage(object? sender, EventArgs e)
    {
        var moduleEnabled = ParentModule.GenericSettings.Enabled.Value;
        var loginMessageEnabled = ParentModule.GenericSettings.NotifyOnLogin.Value;
        var boundByDuty = Condition.IsBoundByDuty();
        var moduleStatus = ParentModule.LogicComponent.GetModuleStatus();

        if (moduleEnabled && loginMessageEnabled && moduleStatus == ModuleStatus.Incomplete && !boundByDuty)
        {
            var moduleName = ParentModule.Name.GetLocalizedString();

            Chat.Print(moduleName, GetStatusMessage());
        }
    }

    DateTime GetNextReset();

    void DoReset();
}