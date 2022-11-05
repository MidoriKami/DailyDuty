using System;
using Dalamud.Logging;

namespace DailyDuty.Configuration.Components;

[Serializable]
public record CharacterData
{
    public string Name = "Unknown";
    public ulong LocalContentID;
    public string World = "UnknownWorld";

    public CharacterData() => Update();

    public void Update()
    {
        var playerData = Service.ClientState.LocalPlayer;

        if (playerData != null)
        {
            LocalContentID = Service.ClientState.LocalContentId;
            Name = playerData.Name.TextValue;
            World = playerData.HomeWorld.GameData?.Name.ToString() ?? "UnknownWorld";
        }
        else
        {
            PluginLog.Warning("Attempted to update CharacterData when LocalPlayer is null.");
        }
    }
}