using System;
using System.Runtime.InteropServices;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
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

        [FieldOffset(0x194)] private readonly byte Flags;

        public bool ObtainedHuntBill => (Flags & 0b0000_0001) != 0;
        public bool ObtainedEliteHuntBill => (Flags & 0b0001_0000) != 0;
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

        [FieldOffset(0x194)] private readonly byte Flags;

        public bool ObtainedLevelOne => (Flags & 0b0000_0010) != 0;
        public bool ObtainedLevelTwo => (Flags & 0b0000_0100) != 0;
        public bool ObtainedLevelThree => (Flags & 0b0000_1000) != 0;
        public bool ObtainedEliteHuntBill => (Flags & 0b0010_0000) != 0;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct StormBloodHunts
    {
        [FieldOffset(0x20)] public readonly byte LevelOneID;
        [FieldOffset(0x21)] public readonly byte LevelTwoID;
        [FieldOffset(0x22)] public readonly byte LevelThreeID;
        [FieldOffset(0x23)] public readonly byte EliteMarkID;

        [FieldOffset(0xA4)] public MarkBillKillCounts LevelOne;
        [FieldOffset(0xB8)] public MarkBillKillCounts LevelTwo;
        [FieldOffset(0xCC)] public MarkBillKillCounts LevelThree;
        [FieldOffset(0xE0)] public readonly int EliteMark;

        [FieldOffset(0x194)] private fixed byte Flags[2];

        public bool ObtainedLevelOne => (Flags[0] & 0b0100_0000) != 0;
        public bool ObtainedLevelTwo => (Flags[0] & 0b1000_0000) != 0;
        public bool ObtainedLevelThree => (Flags[1] & 0b0000_0001) != 0;
        public bool ObtainedEliteHuntBill => (Flags[1] & 0b0000_0010) != 0;
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

        [FieldOffset(0x195)] private readonly byte Flags;

        public bool ObtainedOneNut => (Flags & 0b0000_0100) != 0;
        public bool ObtainedTwoNut => (Flags & 0b0000_1000) != 0;
        public bool ObtainedThreeNut => (Flags & 0b0001_0000) != 0;
        public bool ObtainedEliteHuntBill => (Flags & 0b0010_0000) != 0;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x198)]
    public unsafe struct EndwalkerHunts
    {
        [FieldOffset(0x28)] public readonly byte JuniorID;
        [FieldOffset(0x29)] public readonly byte AssociateID;
        [FieldOffset(0x2A)] public readonly byte SeniorID;
        [FieldOffset(0x2B)] public readonly byte EliteMarkID;

        [FieldOffset(0x144)] public MarkBillKillCounts Junior;
        [FieldOffset(0x158)] public MarkBillKillCounts Associate;
        [FieldOffset(0x16C)] public MarkBillKillCounts Senior;
        [FieldOffset(0x180)] public readonly int EliteMark;

        [FieldOffset(0x195)] private fixed byte Flags[2];

        public bool ObtainedJunior => (Flags[0] & 0b0100_0000) != 0;
        public bool ObtainedAssociate => (Flags[0] & 0b1000_0000) != 0;
        public bool ObtainedSenior => (Flags[1] & 0b0000_0001) != 0;
        public bool ObtainedEliteHuntBill => (Flags[1] & 0b0000_0010) != 0;
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
