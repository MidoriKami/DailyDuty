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
        switch (value)
        {
            case HuntExpansion.RealmReborn:
                return Strings.Common.Expansion.RealmReborn;
            case HuntExpansion.Heavensward:
                return Strings.Common.Expansion.Heavensward;
            case HuntExpansion.Stormblood:
                return Strings.Common.Expansion.Stormblood;
            case HuntExpansion.Shadowbringers:
                return Strings.Common.Expansion.Shadowbringers;
            case HuntExpansion.Endwalker:
                return Strings.Common.Expansion.Endwalker;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
}