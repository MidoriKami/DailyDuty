using DailyDuty.Configuration;
using System;

namespace DailyDuty.System;

internal class ChatManager : IDisposable
{
    public event EventHandler? OnLoginMessage;
    public event EventHandler? OnZoneChangeMessage;

    public ChatManager()
    {
        Service.ClientState.TerritoryChanged += ClientStateOnTerritoryChanged;
        Service.ConfigurationManager.OnCharacterDataAvailable += OnCharacterDataAvailable;

        foreach (var module in Service.ModuleManager.GetLogicComponents())
        {
            OnLoginMessage += module.OnLoginMessage;
            OnZoneChangeMessage += module.OnZoneChangeMessage;
        }
    }

    public void Dispose()
    {
        Service.ClientState.TerritoryChanged -= ClientStateOnTerritoryChanged;
        Service.ConfigurationManager.OnCharacterDataAvailable -= OnCharacterDataAvailable;
    }

    private void OnCharacterDataAvailable(object? sender, CharacterConfiguration e)
    {
        OnLoginMessage?.Invoke(this, EventArgs.Empty);
    }

    private void ClientStateOnTerritoryChanged(object? sender, ushort e)
    {
        OnZoneChangeMessage?.Invoke(this, EventArgs.Empty);
    }
}