using System.Runtime.InteropServices;

namespace DailyDuty.DataStructures;

internal static class ChallengeLogStruct
{
    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    public struct Battles
    {
        [FieldOffset(0x00)] public readonly int DungeonRoulette;
        [FieldOffset(0x04)] public readonly int DungeonMaster;
        [FieldOffset(0x08)] public readonly int Guildhest_1;
        [FieldOffset(0x0C)] public readonly int Guildhest_2;
        [FieldOffset(0x10)] public readonly int WondrousTails;
        [FieldOffset(0x14)] public readonly int Chocobo_1;
        [FieldOffset(0x18)] public readonly int Chocobo_2;
        [FieldOffset(0x1C)] public readonly int Commendations;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x44)]
    public struct CraftingGathering
    {
        [FieldOffset(0x18)] public readonly int unk_1;
        [FieldOffset(0x1C)] public readonly int unk_2;
        [FieldOffset(0x20)] public readonly int unk_3;
        [FieldOffset(0x24)] public readonly int unk_4;
        [FieldOffset(0x28)] public readonly int unk_5;
        [FieldOffset(0x2C)] public readonly int unk_6;
        [FieldOffset(0x30)] public readonly int unk_7;
        [FieldOffset(0x34)] public readonly int unk_8;
        [FieldOffset(0x38)] public readonly int unk_9;
        [FieldOffset(0x3C)] public readonly int unk_10;
        [FieldOffset(0x40)] public readonly int unk_11;
    }
}

//[StructLayout(LayoutKind.Explicit, Size = 0x28)]
//public struct CraftingGathering
//{
//    [FieldOffset(0x00)] public readonly int unk_1;
//    [FieldOffset(0x04)] public readonly int unk_2;
//    [FieldOffset(0x08)] public readonly int unk_3;
//    [FieldOffset(0x0C)] public readonly int unk_4;
//    [FieldOffset(0x10)] public readonly int unk_5;
//    [FieldOffset(0x14)] public readonly int unk_6;
//    [FieldOffset(0x18)] public readonly int unk_7;
//    [FieldOffset(0x1C)] public readonly int unk_8;
//    [FieldOffset(0x20)] public readonly int unk_9;
//    [FieldOffset(0x24)] public readonly int unk_10;
//}