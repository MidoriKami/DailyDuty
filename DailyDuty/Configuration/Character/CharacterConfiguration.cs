using System;
using DailyDuty.Configuration.Character.Components;

namespace DailyDuty.Configuration.Character;

[Serializable]
internal class CharacterConfiguration
{
    public int Version { get; set; } = 1;

    public CharacterData CharacterData = new();
}