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
}
