using System.Runtime.InteropServices;

namespace DailyDuty.DataStructures;

[StructLayout(LayoutKind.Explicit)]
internal struct LevequestStruct
{
    [FieldOffset(0x00)] 
    public readonly int AllowancesRemaining;

    [FieldOffset(0xF8)] 
    public readonly byte LevesAccepted;
}