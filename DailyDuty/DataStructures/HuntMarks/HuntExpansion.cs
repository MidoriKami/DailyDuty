using System;
using DailyDuty.Localization;

namespace DailyDuty.DataStructures.HuntMarks;

public enum HuntExpansion
{    
    RealmReborn,
    Heavensward,
    Stormblood,
    Shadowbringers,
    Endwalker
}

public static class ExpansionExtensions
{
    public static string GetTranslatedString(this HuntExpansion value)
    {
        return value switch
        {
            HuntExpansion.RealmReborn => Strings.Expansion_RealmReborn,
            HuntExpansion.Heavensward => Strings.Expansion_Heavensward,
            HuntExpansion.Stormblood => Strings.Expansion_Stormblood,
            HuntExpansion.Shadowbringers => Strings.Expansion_Shadowbringers,
            HuntExpansion.Endwalker => Strings.Expansion_Endwalker,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}