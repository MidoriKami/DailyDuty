using System;
using System.ComponentModel;
using DailyDuty.Localization;
using Dalamud.Utility;

namespace DailyDuty.Enums
{
    public enum ExpansionType
    {    
        RealmReborn,
        Heavensward,
        Stormblood,
        Shadowbringers,
        Endwalker
    }

    public static class ExpansionTypeExtensions
    {
        public static string Description(this ExpansionType value)
        {
            switch (value)
            {
                case ExpansionType.RealmReborn:
                    return Strings.Common.RealmRebornLabel;
                case ExpansionType.Heavensward:
                    return Strings.Common.HeavenswardLabel;
                case ExpansionType.Stormblood:
                    return Strings.Common.StormbloodLabel;
                case ExpansionType.Shadowbringers:
                    return Strings.Common.ShadowbringersLabel;
                case ExpansionType.Endwalker:
                    return Strings.Common.EndwalkerLabel;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
