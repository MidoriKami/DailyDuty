using System;
using System.Collections.Generic;
using DailyDuty.Data.SettingsObjects;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace DailyDuty.Data;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public SystemSettings System = new();
    public CountdownTimerSettings TimerSettings = new();

    public Dictionary<ulong, CharacterSettings> CharacterSettingsMap = new();
    public ulong CurrentCharacter = new();

    public CharacterSettings Current()
    {
        return CharacterSettingsMap[CurrentCharacter];
    }

    public void UpdateCharacter()
    {
        var newCharacterID = Service.ClientState.LocalContentId;

        if (CharacterSettingsMap.ContainsKey(newCharacterID))
        {
            CurrentCharacter = newCharacterID;
        }
        else
        {
            var localPlayer = Service.ClientState.LocalPlayer;
            if (localPlayer != null)
            {
                CharacterSettingsMap.Add(newCharacterID, new CharacterSettings());
                CharacterSettingsMap[newCharacterID].CharacterName = localPlayer.Name.ToString();
                CurrentCharacter = newCharacterID;
            }
        }

        Save();
    }

    [NonSerialized]
    private DalamudPluginInterface? pluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;

        CharacterSettingsMap ??= new();

        if (!CharacterSettingsMap.ContainsKey(0))
        {
            CharacterSettingsMap.Add(0, new());
        }

        Save();
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}