using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DailyDuty.Configuration;
using DailyDuty.UserInterface.OverlayWindows;
using Dalamud.Game;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using KamiLib;

namespace DailyDuty.System;

internal class ConfigurationManager : IDisposable
{
    private readonly CharacterConfiguration nullCharacterConfiguration = new();

    private CharacterConfiguration? backingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? backingCharacterConfiguration : nullCharacterConfiguration;

    private bool LoggedIn => Service.ClientState.LocalPlayer != null && Service.ClientState.LocalContentId != 0;
    
    [MemberNotNullWhen(returnValue: true, nameof(backingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataLoaded;
    public event EventHandler? OnCharacterDataUnloaded;

    public ConfigurationManager()
    {
        if (LoggedIn)
        {
            LoadCharacterConfiguration();
        }

        OnCharacterDataLoaded += LoadOverlayWindows;
        OnCharacterDataUnloaded += UnloadOverlayWindows;
        
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
    }

    public void Dispose()
    {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        
        OnCharacterDataLoaded -= LoadOverlayWindows;
        OnCharacterDataUnloaded -= UnloadOverlayWindows;
    }

    private void OnLogin(object? sender, EventArgs e)
    {
        PluginLog.Verbose("Adding Login Listener");

        Service.Framework.Update += LoginLogic;
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        PluginLog.Verbose($"Logging out of '{CharacterConfiguration.CharacterData.Name}'");

        CharacterDataLoaded = false;
        OnCharacterDataUnloaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoginLogic(Framework framework)
    {
        if (!LoggedIn) return;

        Service.Framework.RunOnTick(LoadCharacterConfiguration, TimeSpan.FromSeconds(1));

        PluginLog.Verbose("Removing Login Listener");
        Service.Framework.Update -= LoginLogic;
    }

    private void LoadCharacterConfiguration()
    {
        PluginLog.Verbose($"Logging into Character '{Service.ClientState.LocalPlayer?.Name.TextValue}'");

        backingCharacterConfiguration = CharacterConfiguration.Load(Service.ClientState.LocalContentId);
        
        CharacterDataLoaded = true;
        OnCharacterDataLoaded?.Invoke(this, CharacterConfiguration);
    }

    public void Save()
    {
        if (CharacterDataLoaded)
        {
            CharacterConfiguration.Save();
        }
    }
    
    private void LoadOverlayWindows(object? sender, CharacterConfiguration e)
    {
        KamiCommon.WindowManager.AddWindow(new TimersOverlayWindow());
        KamiCommon.WindowManager.AddWindow(new TodoOverlayWindow());
    }
    
    private void UnloadOverlayWindows(object? sender, EventArgs e)
    {
        var windowList = new List<Window>();
        
        windowList.AddRange(KamiCommon.WindowManager.GetWindows().OfType<TimersOverlayWindow>());
        windowList.AddRange(KamiCommon.WindowManager.GetWindows().OfType<TodoOverlayWindow>());
        
        foreach (var overlay in windowList)
        {
            KamiCommon.WindowManager.RemoveWindow(overlay);
        }
    }
}