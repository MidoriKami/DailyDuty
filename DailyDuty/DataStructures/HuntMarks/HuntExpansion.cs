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
            HuntExpansion.RealmReborn => Strings.Common.Expansion.RealmReborn,
            HuntExpansion.Heavensward => Strings.Common.Expansion.Heavensward,
            HuntExpansion.Stormblood => Strings.Common.Expansion.Stormblood,
            HuntExpansion.Shadowbringers => Strings.Common.Expansion.Shadowbringers,
            HuntExpansion.Endwalker => Strings.Common.Expansion.Endwalker,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}