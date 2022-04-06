using System;
using System.Runtime.InteropServices;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    [Flags]
    public enum RealmRebornFlags : int
    {
        HuntBill = 0x000001,
        Elite = 0x000010
    }

    [Flags]
    public enum HeavenswardFlags : int
    {
        LevelOne = 0x000002,
        LevelTwo = 0x000004,
        LevelThree = 0x000008,
        Elite = 0x000020
    }

    [Flags]
    public enum StormbloodFlags : int
    {
        LevelOne = 0x000040,
        LevelTwo = 0x000080,
        LevelThree = 0x000100,
        Elite = 0x000200
    }

    [Flags]
    public enum ShadowbringersFlags : int
    {
        OneNut = 0x000400,
        TwoNut = 0x000800,
        ThreeNut = 0x001000,
        Elite = 0x002000
    }

    [Flags]
    public enum EndwalkerFlags
    {
        Junior = 0x004000,
        Associate = 0x008000,
        Senior = 0x010000,
        Elite = 0x020000
    }

    public static class HuntFlags
    {
        public static RealmRebornFlags RealmReborn;
        public static HeavenswardFlags Heavensward;
        public static StormbloodFlags Stormblood;
        public static ShadowbringersFlags Shadowbringers;
        public static EndwalkerFlags Endwalker;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x14)]
    public unsafe struct MarkBillKillCounts
    {
        [FieldOffset(0x00)] public readonly int First;
        [FieldOffset(0x04)] public readonly int Second;
        [FieldOffset(0x08)] public readonly int Third;
        [FieldOffset(0x0C)] public readonly int Fourth;
        [FieldOffset(0x10)] public readonly int Fifth;

        [FieldOffset(0x00)] public fixed int Raw[5];
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public struct RealmRebornHunts
    {
        [FieldOffset(0x1A)] public readonly byte HuntMarkID;
        [FieldOffset(0x1E)] public readonly byte EliteMarkID;

        [FieldOffset(0x2C)] public MarkBillKillCounts KillCounts;
        [FieldOffset(0x7C)] public readonly int EliteMark;

        [FieldOffset(0x194)] public readonly RealmRebornFlags Flags;

        public bool ObtainedHuntBill => Flags.HasFlag(RealmRebornFlags.HuntBill);
        public bool ObtainedEliteHuntBill => Flags.HasFlag(RealmRebornFlags.Elite);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public struct HeavensWardHunts
    {
        [FieldOffset(0x1B)] public readonly byte LevelOneID;
        [FieldOffset(0x1C)] public readonly byte LevelTwoID;
        [FieldOffset(0x1D)] public readonly byte LevelThreeID;
        [FieldOffset(0x1F)] public readonly byte EliteMarkID;

        [FieldOffset(0x40)] public MarkBillKillCounts LevelOne;
        [FieldOffset(0x54)] public MarkBillKillCounts LevelTwo;
        [FieldOffset(0x68)] public MarkBillKillCounts LevelThree;
        [FieldOffset(0x90)] public readonly int EliteMark;

        [FieldOffset(0x194)] public readonly HeavenswardFlags Flags;

        public bool ObtainedLevelOne => Flags.HasFlag(HeavenswardFlags.LevelOne);
        public bool ObtainedLevelTwo => Flags.HasFlag(HeavenswardFlags.LevelTwo);
        public bool ObtainedLevelThree => Flags.HasFlag(HeavenswardFlags.LevelThree);
        public bool ObtainedEliteHuntBill => Flags.HasFlag(HeavenswardFlags.Elite);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public struct StormBloodHunts
    {
        [FieldOffset(0x20)] public readonly byte LevelOneID;
        [FieldOffset(0x21)] public readonly byte LevelTwoID;
        [FieldOffset(0x22)] public readonly byte LevelThreeID;
        [FieldOffset(0x23)] public readonly byte EliteMarkID;

        [FieldOffset(0xA4)] public MarkBillKillCounts LevelOne;
        [FieldOffset(0xB8)] public MarkBillKillCounts LevelTwo;
        [FieldOffset(0xCC)] public MarkBillKillCounts LevelThree;
        [FieldOffset(0xE0)] public readonly int EliteMark;

        [FieldOffset(0x194)] private readonly StormbloodFlags Flags;

        public bool ObtainedLevelOne => Flags.HasFlag(StormbloodFlags.LevelOne);
        public bool ObtainedLevelTwo => Flags.HasFlag(StormbloodFlags.LevelTwo);
        public bool ObtainedLevelThree => Flags.HasFlag(StormbloodFlags.LevelThree);
        public bool ObtainedEliteHuntBill => Flags.HasFlag(StormbloodFlags.Elite);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public struct ShadowBringersHunts
    {
        [FieldOffset(0x24)] public readonly byte OneNutID;
        [FieldOffset(0x25)] public readonly byte TwoNutID;
        [FieldOffset(0x26)] public readonly byte ThreeNutID;
        [FieldOffset(0x27)] public readonly byte EliteMarkID;

        [FieldOffset(0xF4)] public MarkBillKillCounts OneNut;
        [FieldOffset(0x108)] public MarkBillKillCounts TwoNut;
        [FieldOffset(0x11C)] public MarkBillKillCounts ThreeNut;
        [FieldOffset(0x130)] public readonly int EliteMark;

        [FieldOffset(0x194)] private readonly ShadowbringersFlags Flags;

        public bool ObtainedOneNut => Flags.HasFlag(ShadowbringersFlags.OneNut);
        public bool ObtainedTwoNut => Flags.HasFlag(ShadowbringersFlags.TwoNut);
        public bool ObtainedThreeNut => Flags.HasFlag(ShadowbringersFlags.ThreeNut);
        public bool ObtainedEliteHuntBill => Flags.HasFlag(ShadowbringersFlags.Elite);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public struct EndwalkerHunts
    {
        [FieldOffset(0x28)] public readonly byte JuniorID;
        [FieldOffset(0x29)] public readonly byte AssociateID;
        [FieldOffset(0x2A)] public readonly byte SeniorID;
        [FieldOffset(0x2B)] public readonly byte EliteMarkID;

        [FieldOffset(0x144)] public MarkBillKillCounts Junior;
        [FieldOffset(0x158)] public MarkBillKillCounts Associate;
        [FieldOffset(0x16C)] public MarkBillKillCounts Senior;
        [FieldOffset(0x180)] public readonly int EliteMark;

        [FieldOffset(0x194)] public readonly EndwalkerFlags Flags;

        public bool ObtainedJunior => Flags.HasFlag(EndwalkerFlags.Junior);
        public bool ObtainedAssociate => Flags.HasFlag(EndwalkerFlags.Associate);
        public bool ObtainedSenior => Flags.HasFlag(EndwalkerFlags.Senior);
        public bool ObtainedEliteHuntBill => Flags.HasFlag(EndwalkerFlags.Elite);
    }

    //[Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct MobHuntStruct
    {
        [FieldOffset(0x00)] public fixed byte Raw[0x198];

        [FieldOffset(0x00)] public RealmRebornHunts RealmReborn;
        [FieldOffset(0x00)] public HeavensWardHunts HeavensWard;
        [FieldOffset(0x00)] public StormBloodHunts StormBlood;
        [FieldOffset(0x00)] public ShadowBringersHunts ShadowBringers;
        [FieldOffset(0x00)] public EndwalkerHunts Endwalker;
    }
}
