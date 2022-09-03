using System;
using System.Diagnostics.CodeAnalysis;
using DailyDuty.Configuration;
using DailyDuty.Utilities;
using Dalamud.Game;

namespace DailyDuty.System;

internal class ConfigurationManager : IDisposable
{
    private readonly CharacterConfiguration nullCharacterConfiguration = new();

    private CharacterConfiguration? backingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? backingCharacterConfiguration : nullCharacterConfiguration;

    private bool LoggedIn => Service.ClientState.LocalPlayer != null && Service.ClientState.LocalContentId != 0;
    
    [MemberNotNullWhen(returnValue: true, nameof(backingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataAvailable;

    public ConfigurationManager()
    {
        if (LoggedIn)
        {
            LoadCharacterConfiguration();
        }

        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
    }

    public void Dispose()
    {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
    }

    private void OnLogin(object? sender, EventArgs e)
    {
        Log.Verbose("Adding Login Listener");

        Service.Framework.Update += LoginLogic;
    }

    private void OnLogout(object? sender, EventArgs e)
    {
        Log.Verbose($"Logging out of '{CharacterConfiguration.CharacterData.Name}'");

        CharacterDataLoaded = false;
    }

    private void LoginLogic(Framework framework)
    {
        if (!LoggedIn) return;

        Service.Framework.RunOnTick(LoadCharacterConfiguration, TimeSpan.FromSeconds(1));

        Log.Verbose("Removing Login Listener");
        Service.Framework.Update -= LoginLogic;
    }

    private void LoadCharacterConfiguration()
    {
        Log.Verbose($"Logging into Character '{Service.ClientState.LocalPlayer?.Name.TextValue}'");

        backingCharacterConfiguration = CharacterConfiguration.Load(Service.ClientState.LocalContentId);
        CharacterDataLoaded = true;

        OnCharacterDataAvailable?.Invoke(this, CharacterConfiguration);
    }

    public void Save()
    {
        CharacterConfiguration.Save();
    }
}