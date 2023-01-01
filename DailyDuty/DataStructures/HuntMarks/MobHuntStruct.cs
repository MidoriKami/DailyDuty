using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace DailyDuty.DataStructures.HuntMarks;

//[Signature("D1 48 8D 0D ?? ?? ?? ?? 48 83 C4 20 5F E9 ?? ?? ?? ??", ScanType = ScanType.StaticAddress)]
[StructLayout(LayoutKind.Explicit, Size = 0x198)]
public unsafe struct MobHuntStruct
{
    [FieldOffset(0x00)] private fixed byte Raw[0x198];

    [FieldOffset(0x1A)] public fixed byte MarkID[18];
    [FieldOffset(0x2C)] private fixed int CurrentKills[18 * 5];
    [FieldOffset(0x194)] private readonly int Flags;

    public BitArray Obtained => new(BitConverter.GetBytes(Flags));
    public KillCountArray KillCounts
    {
        get
        {
            fixed (int* killArray = CurrentKills)
                return new KillCountArray(killArray);
        }
    }
}
