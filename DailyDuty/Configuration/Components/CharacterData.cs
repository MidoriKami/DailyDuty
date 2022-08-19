using System;

namespace DailyDuty.Configuration.Components;

[Serializable]
public record CharacterData
{
    public string Name = "Unknown";
    public ulong LocalContentID = 0;
    public string World = "UnknownWorld";
}