using System;
using System.Runtime.InteropServices;
using DailyDuty.Configuration.Character.Enums;
using DailyDuty.System.Localization;
using Lumina.Excel.GeneratedSheets;
using Expansion = DailyDuty.Configuration.Character.Enums.Expansion;

namespace DailyDuty.DataStructures;

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

public class HuntData
{
    public byte HuntID { get; init; }
    public HuntMarkType HuntType { get; init; }
    public KillCounts KillCounts { get; init; } = new();
    public HuntInfo TargetInfo
    {
        get
        {
            var orderTypeSheet = Service.DataManager.GetExcelSheet<MobHuntOrderType>()!;
            var huntOrderSheet = Service.DataManager.GetExcelSheet<MobHuntOrder>()!;

            var indexOffset = orderTypeSheet.GetRow((uint)HuntType)!.OrderStart.Row;
            var targetRow = indexOffset + HuntID - 1;

            if (HuntID == 0)
            {
                return new HuntInfo();
            }

            if (IsElite)
            {
                return new HuntInfo
                {
                    [0] = huntOrderSheet.GetRow(targetRow, 0)!,
                };
            }
            else
            {
                return new HuntInfo
                {
                    [0] = huntOrderSheet.GetRow(targetRow, 0)!,
                    [1] = huntOrderSheet.GetRow(targetRow, 1)!,
                    [2] = huntOrderSheet.GetRow(targetRow, 2)!,
                    [3] = huntOrderSheet.GetRow(targetRow, 3)!,
                    [4] = huntOrderSheet.GetRow(targetRow, 4)!,
                };
            }
        }
    }

    public bool Obtained { get; init; }
    public bool IsElite => HuntType is
        HuntMarkType.EndwalkerElite or
        HuntMarkType.ShadowbringersElite or
        HuntMarkType.StormbloodElite or
        HuntMarkType.HeavenswardElite or
        HuntMarkType.RealmRebornElite;
}

public class HuntInfo
{
    private MobHuntOrder?[] Raw { get; set; } = new MobHuntOrder[5];

    public string? FirstName => Raw[0]?.Target.Value?.Name.Value?.Singular.RawString;
    public string? SecondName => Raw[1]?.Target.Value?.Name.Value?.Singular.RawString;
    public string? ThirdName => Raw[2]?.Target.Value?.Name.Value?.Singular.RawString;
    public string? FourthName => Raw[3]?.Target.Value?.Name.Value?.Singular.RawString;
    public string? FifthName => Raw[4]?.Target.Value?.Name.Value?.Singular.RawString;

    public MobHuntOrder? this[int i]
    {
        get => Raw[i];
        init => Raw[i] = value;
    }
}

public class KillCounts
{
    private int[] Raw { get; set; } = new int[5];

    public int First => Raw[0];
    public int Second => Raw[1];
    public int Third => Raw[2];
    public int Fourth => Raw[3];
    public int Fifth => Raw[4];

    public int this[int i]
    {
        get => Raw[i];
        set => Raw[i] = value;
    }

}

//[Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
[StructLayout(LayoutKind.Explicit, Size = 0x198)]
public unsafe struct MobHuntStruct
{
    [FieldOffset(0x00)] private fixed byte Raw[0x198];

    [FieldOffset(0x1A)] private fixed byte ID[18];
    [FieldOffset(0x2C)] private fixed int CurrentKills[18 * 5];
    [FieldOffset(0x194)] private readonly int Flags;

    public HuntData Get(HuntMarkType type)
    {
        int index = (int)type;

        return new HuntData
        {
            KillCounts = new KillCounts
            {
                [0] = CurrentKills[index * 5 + 0],
                [1] = CurrentKills[index * 5 + 1],
                [2] = CurrentKills[index * 5 + 2],
                [3] = CurrentKills[index * 5 + 3],
                [4] = CurrentKills[index * 5 + 4]
            },
            HuntID = ID[index],
            Obtained = (Flags & 1 << index) != 0,
            HuntType = type
        };
    }
}

public static class HuntMarkTypeExtensions
{
    public static string GetLabel(this HuntMarkType value)
    {
        var expansionLabel = value.GetExpansion().GetLocalizedString();
        var level = value.GetLevel();

        return expansionLabel + " " + level;
    }

    public static Expansion GetExpansion(this HuntMarkType type)
    {
        switch (type)
        {
            case HuntMarkType.RealmRebornElite:
            case HuntMarkType.RealmRebornLevelOne:
                return Expansion.RealmReborn;

            case HuntMarkType.HeavenswardLevelOne:
            case HuntMarkType.HeavenswardLevelTwo:
            case HuntMarkType.HeavenswardLevelThree:
            case HuntMarkType.HeavenswardElite:
                return Expansion.Heavensward;

            case HuntMarkType.StormbloodLevelOne:
            case HuntMarkType.StormbloodLevelTwo:
            case HuntMarkType.StormbloodLevelThree:
            case HuntMarkType.StormbloodElite:
                return Expansion.Stormblood;

            case HuntMarkType.ShadowbringersLevelOne:
            case HuntMarkType.ShadowbringersLevelTwo:
            case HuntMarkType.ShadowbringersLevelThree:
            case HuntMarkType.ShadowbringersElite:
                return Expansion.Shadowbringers;

            case HuntMarkType.EndwalkerLevelOne:
            case HuntMarkType.EndwalkerLevelTwo:
            case HuntMarkType.EndwalkerLevelThree:
            case HuntMarkType.EndwalkerElite:
                return Expansion.Endwalker;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static string GetLevel(this HuntMarkType type)
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