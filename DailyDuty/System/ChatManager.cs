using DailyDuty.Configuration;
using System;
using System.Diagnostics;
using DailyDuty.Utilities;

namespace DailyDuty.System;

internal class ChatManager : IDisposable
{
    public event EventHandler? OnLoginMessage;
    public event EventHandler? OnZoneChangeMessage;

    private readonly Stopwatch stopwatch = new();


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
        ResetManager.ResetModules();

        OnLoginMessage?.Invoke(this, EventArgs.Empty);
        stopwatch.Restart();
    }
    
    private void ClientStateOnTerritoryChanged(object? sender, ushort e)
    {
        if (stopwatch.Elapsed.Minutes >= 5 || stopwatch.IsRunning == false)
        {
            stopwatch.Restart();
            OnZoneChangeMessage?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            var lockoutRemaining = TimeSpan.FromMinutes(5) - stopwatch.Elapsed;
            Log.Verbose($"Zone Change Messages Suppressed, '{lockoutRemaining}' Remaining");
        }
    }
}