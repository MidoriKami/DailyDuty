using System.Runtime.InteropServices;

namespace DailyDuty.DataStructures;

[StructLayout(LayoutKind.Explicit)]
internal struct DomanEnclaveStruct
{
    [FieldOffset(0xA0)] 
    public readonly ushort Donated;

    [FieldOffset(0xA6)] 
    public readonly ushort Allowance;
}