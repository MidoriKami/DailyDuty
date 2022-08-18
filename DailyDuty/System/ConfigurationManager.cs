using System;
using System.Diagnostics.CodeAnalysis;
using DailyDuty.Configuration.Character;
using DailyDuty.Configuration.System;
using DailyDuty.Utilities;
using Dalamud.Game;

namespace DailyDuty.System;

internal class ConfigurationManager : IDisposable
{
    public SystemConfiguration SystemConfiguration { get; }

    private readonly CharacterConfiguration nullCharacterConfiguration = new();

    private CharacterConfiguration? backingCharacterConfiguration;

    public CharacterConfiguration CharacterConfiguration => CharacterDataLoaded ? backingCharacterConfiguration : nullCharacterConfiguration;

    private bool LoggedIn => Service.ClientState.LocalPlayer != null && Service.ClientState.LocalContentId != 0;
    
    [MemberNotNullWhen(returnValue: true, nameof(backingCharacterConfiguration))]
    public bool CharacterDataLoaded { get; private set; }

    public event EventHandler<CharacterConfiguration>? OnCharacterDataAvailable;

    public ConfigurationManager()
    {
        Log.Verbose("Constructing ConfigurationManager");

        SystemConfiguration = SystemConfiguration.Load();

        if (LoggedIn)
        {
            LoadCharacterConfiguration();
        }

        Service.ClientState.Login += Login;
        Service.ClientState.Logout += Logout;
    }

    public void Dispose()
    {
        Service.ClientState.Login -= Login;
        Service.ClientState.Logout -= Logout;

        SaveAll();
    }

    private void Login(object? sender, EventArgs e)
    {
        Log.Verbose("Adding Login Listener");

        Service.Framework.Update += LoginLogic;
    }

    private void Logout(object? sender, EventArgs e)
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

    public void SaveAll()
    {
        SystemConfiguration.Save();
        CharacterConfiguration.Save();
    }
}