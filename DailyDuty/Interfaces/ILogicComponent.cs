using System;
using DailyDuty.DataModels;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Utilities;

namespace DailyDuty.Interfaces;

public interface ILogicComponent : IDisposable
{
    IModule ParentModule { get; }

    ModuleStatus GetModuleStatus();

    string GetStatusMessage();

    DalamudLinkPayload? DalamudLinkPayload { get; }
    bool LinkPayloadActive { get; }

    void OnLoginMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnLogin.Value) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Complete) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Unavailable) return;

        var moduleName = ParentModule.Name.GetTranslatedString();

        var statusMessage = GetStatusMessage();

        if(statusMessage == string.Empty) return;
        Chat.Print(moduleName, GetStatusMessage(), LinkPayloadActive ? DalamudLinkPayload : null);
    }        
        
    void OnZoneChangeMessage(object? sender, EventArgs e)
    {
        if (!ParentModule.GenericSettings.Enabled.Value) return;
        if (!ParentModule.GenericSettings.NotifyOnZoneChange.Value) return;
        if (Condition.IsBoundByDuty()) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Complete) return;
        if (ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Unavailable) return;

        var moduleName = ParentModule.Name.GetTranslatedString();

        var statusMessage = GetStatusMessage();

        if(statusMessage == string.Empty) return;
        Chat.Print(moduleName, GetStatusMessage(), LinkPayloadActive ? DalamudLinkPayload : null);
    }

    DateTime GetNextReset();

    void DoReset();
}