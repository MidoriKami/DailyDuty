using DailyDuty.Modules.Enums;
using System;
using DailyDuty.Utilities;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace DailyDuty.Interfaces;

internal interface ILogicComponent : IDisposable
{
    IModule ParentModule { get; }

    ModuleStatus GetModuleStatus();

    string GetStatusMessage();

    DalamudLinkPayload? DalamudLinkPayload { get; }

    void OnLoginMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnLogin.Value) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Complete) return;

        var moduleName = ParentModule.Name.GetLocalizedString();

        Chat.Print(moduleName, GetStatusMessage(), DalamudLinkPayload);
    }        
        
    void OnZoneChangeMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnZoneChange.Value) return;
        if (Condition.IsBoundByDuty()) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Complete) return;

        var moduleName = ParentModule.Name.GetLocalizedString();

        Chat.Print(moduleName, GetStatusMessage(), DalamudLinkPayload);
    }

    DateTime GetNextReset();

    void DoReset();
}