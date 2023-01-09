using System;
using DailyDuty.DataModels;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.ChatCommands;
using KamiLib.GameState;

namespace DailyDuty.Interfaces;

public interface ILogicComponent : IDisposable
{
    IModule ParentModule { get; }

    ModuleStatus GetModuleStatus();

    ModuleStatus Status() => ParentModule.GenericSettings.Suppressed ? ModuleStatus.Suppressed : GetModuleStatus();

    string GetStatusMessage();

    DalamudLinkPayload? DalamudLinkPayload { get; }
    bool LinkPayloadActive { get; }

    void OnLoginMessage(object? sender, EventArgs e)
    {
        if (ParentModule.GenericSettings is not { Enabled.Value: true, NotifyOnLogin.Value: true }) return;
        if (ParentModule.GenericSettings.Suppressed) return;
        if (ParentModule.LogicComponent.GetModuleStatus() is not ModuleStatus.Incomplete or ModuleStatus.Unknown) return;

        var moduleName = ParentModule.Name.GetTranslatedString();

        var statusMessage = GetStatusMessage();

        if(statusMessage == string.Empty) return;
        Chat.Print(moduleName, GetStatusMessage(), LinkPayloadActive ? DalamudLinkPayload : null);
    }        
        
    void OnZoneChangeMessage(object? sender, EventArgs e)
    {
        if (ParentModule.GenericSettings is not { Enabled.Value: true, NotifyOnZoneChange.Value: true }) return;
        if (ParentModule.GenericSettings.Suppressed) return;
        if (Condition.IsBoundByDuty()) return;
        if (ParentModule.LogicComponent.GetModuleStatus() is not ModuleStatus.Incomplete or ModuleStatus.Unknown) return;

        var moduleName = ParentModule.Name.GetTranslatedString();

        var statusMessage = GetStatusMessage();

        if(statusMessage == string.Empty) return;
        Chat.Print(moduleName, GetStatusMessage(), LinkPayloadActive ? DalamudLinkPayload : null);
    }

    DateTime GetNextReset();

    void DoReset();

    void Reset()
    {
        ParentModule.GenericSettings.Suppressed.Value = false;
        DoReset();
    }
}