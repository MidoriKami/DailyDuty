using System.ComponentModel;
using System.Runtime.InteropServices;
using Dalamud.Utility;

// ReSharper disable InconsistentNaming

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public enum HuntMarkType
    {
        [Description("Level 1 :: A Realm Reborn")] RealmReborn_LevelOne = 0,
        [Description("Level 1 :: Heavensward")] Heavensward_LevelOne = 1,
        [Description("Level 2 :: Heavensward")] Heavensward_LevelTwo = 2,
        [Description("Level 3 :: Heavensward")] Heavensward_LevelThree = 3,
        [Description("Elite :: A Realm Reborn")] RealmReborn_Elite = 4,
        [Description("Elite :: Heavensward")] Heavensward_Elite = 5,
        [Description("Level 1 :: Stormblood")] Stormblood_LevelOne = 6,
        [Description("Level 2 :: Stormblood")] Stormblood_LevelTwo = 7,
        [Description("Level 3 :: Stormblood")] Stormblood_LevelThree = 8,
        [Description("Elite :: Stormblood")] Stormblood_Elite = 9,
        [Description("Level 1 :: Shadowbringers")] Shadowbringers_LevelOne = 10,
        [Description("Level 2 :: Shadowbringers")] Shadowbringers_LevelTwo = 11,
        [Description("Level 3 :: Shadowbringers")] Shadowbringers_LevelThree = 12,
        [Description("Elite :: Shadowbringers")] Shadowbringers_Elite = 13,
        [Description("Level 1 :: Endwalker")] Endwalker_LevelOne = 14,
        [Description("Level 2 :: Endwalker")] Endwalker_LevelTwo = 15,
        [Description("Level 3 :: Endwalker")] Endwalker_LevelThree = 16,
        [Description("Elite :: Endwalker")] Endwalker_Elite = 17
    }

    public class HuntData
    {
        public byte HuntID { get; init; }
        public HuntMarkType HuntType { get; init; }
        public KillCounts KillCounts { get; init; } = new();
        public bool Obtained { get; init; }
    }

    public class KillCounts
    {
        public int First { get; init; }
        public int Second { get; init; }
        public int Third { get; init; }
        public int Fourth { get; init; }
        public int Fifth { get; init; }
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

            return new HuntData()
            {
                KillCounts = new KillCounts()
                {
                    First = CurrentKills[(index * 5) + 0],
                    Second = CurrentKills[(index * 5) + 1],
                    Third = CurrentKills[(index * 5) + 2],
                    Fourth = CurrentKills[(index * 5) + 3],
                    Fifth = CurrentKills[(index * 5) + 4]
                },
                HuntID = ID[index],
                Obtained = (Flags & (1 << index)) != 0,
                HuntType = type
            };
        }
    }

    public static class HuntMarkTypeExtensions
    {
        public static string Description(this HuntMarkType value)
        {
            return value.GetAttribute<DescriptionAttribute>().Description;
        }
    }
}
