using System;

namespace DailyDuty.DataModels;

[Serializable]
public record CharacterData
{
    public string Name = "Unknown";
    public ulong LocalContentID = 0;
    public string World = "UnknownWorld";
}