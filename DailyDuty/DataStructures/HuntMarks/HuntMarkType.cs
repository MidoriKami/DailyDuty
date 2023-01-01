using System;
using DailyDuty.Localization;

namespace DailyDuty.DataStructures.HuntMarks;

public enum HuntMarkType
{
    RealmRebornLevelOne = 0,
    HeavenswardLevelOne = 1,
    HeavenswardLevelTwo = 2,
    HeavenswardLevelThree = 3,
    RealmRebornElite = 4,
    HeavenswardElite = 5,
    StormbloodLevelOne = 6,
    StormbloodLevelTwo = 7,
    StormbloodLevelThree = 8,
    StormbloodElite = 9,
    ShadowbringersLevelOne = 10,
    ShadowbringersLevelTwo = 11,
    ShadowbringersLevelThree = 12,
    ShadowbringersElite = 13,
    EndwalkerLevelOne = 14,
    EndwalkerLevelTwo = 15,
    EndwalkerLevelThree = 16,
    EndwalkerElite = 17
}

public static class HuntMarkTypeExtensions
{
    public static string GetLabel(this HuntMarkType value)
    {
        var expansionLabel = value.GetExpansion().GetTranslatedString();
        var level = value.GetLevel();

        return expansionLabel + " " + level;
    }

    private static HuntExpansion GetExpansion(this HuntMarkType type)
    {
        switch (type)
        {
            case HuntMarkType.RealmRebornElite:
            case HuntMarkType.RealmRebornLevelOne:
                return HuntExpansion.RealmReborn;

            case HuntMarkType.HeavenswardLevelOne:
            case HuntMarkType.HeavenswardLevelTwo:
            case HuntMarkType.HeavenswardLevelThree:
            case HuntMarkType.HeavenswardElite:
                return HuntExpansion.Heavensward;

            case HuntMarkType.StormbloodLevelOne:
            case HuntMarkType.StormbloodLevelTwo:
            case HuntMarkType.StormbloodLevelThree:
            case HuntMarkType.StormbloodElite:
                return HuntExpansion.Stormblood;

            case HuntMarkType.ShadowbringersLevelOne:
            case HuntMarkType.ShadowbringersLevelTwo:
            case HuntMarkType.ShadowbringersLevelThree:
            case HuntMarkType.ShadowbringersElite:
                return HuntExpansion.Shadowbringers;

            case HuntMarkType.EndwalkerLevelOne:
            case HuntMarkType.EndwalkerLevelTwo:
            case HuntMarkType.EndwalkerLevelThree:
            case HuntMarkType.EndwalkerElite:
                return HuntExpansion.Endwalker;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private static string GetLevel(this HuntMarkType type)
    {
        switch (type)
        {
            case HuntMarkType.RealmRebornLevelOne:
            case HuntMarkType.HeavenswardLevelOne:
            case HuntMarkType.StormbloodLevelOne:
            case HuntMarkType.ShadowbringersLevelOne:
            case HuntMarkType.EndwalkerLevelOne:
                return Strings.Module.HuntMarks.LevelOne;

            case HuntMarkType.ShadowbringersLevelTwo:
            case HuntMarkType.EndwalkerLevelTwo:
            case HuntMarkType.HeavenswardLevelTwo:
            case HuntMarkType.StormbloodLevelTwo:
                return Strings.Module.HuntMarks.LevelTwo;


            case HuntMarkType.StormbloodLevelThree:
            case HuntMarkType.ShadowbringersLevelThree:
            case HuntMarkType.EndwalkerLevelThree:
            case HuntMarkType.HeavenswardLevelThree:
                return Strings.Module.HuntMarks.LevelThree;


            case HuntMarkType.HeavenswardElite:
            case HuntMarkType.StormbloodElite:
            case HuntMarkType.EndwalkerElite:
            case HuntMarkType.ShadowbringersElite:
            case HuntMarkType.RealmRebornElite:
                return Strings.Module.HuntMarks.Elite;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}