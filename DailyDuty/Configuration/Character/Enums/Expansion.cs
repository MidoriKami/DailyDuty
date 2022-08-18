using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Configuration.Character.Enums;

public enum Expansion
{    
    RealmReborn,
    Heavensward,
    Stormblood,
    Shadowbringers,
    Endwalker
}

public static class ExpansionExtensions
{
    public static string GetLocalizedString(this Expansion value)
    {
        switch (value)
        {
            case Expansion.RealmReborn:
                return Strings.Common.Expansion.RealmReborn;
            case Expansion.Heavensward:
                return Strings.Common.Expansion.Heavensward;
            case Expansion.Stormblood:
                return Strings.Common.Expansion.Stormblood;
            case Expansion.Shadowbringers:
                return Strings.Common.Expansion.Shadowbringers;
            case Expansion.Endwalker:
                return Strings.Common.Expansion.Endwalker;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
}