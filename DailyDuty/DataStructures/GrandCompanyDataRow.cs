using System.Runtime.InteropServices;
using Dalamud.Logging;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;

namespace DailyDuty.DataStructures;

public unsafe class GrandCompanyDataArray
{
    private readonly GrandCompanyDataRow* dataRows;

    public GrandCompanyDataArray(nint supplyAgentPointer)
    {
        // Offset 104 bytes is a pointer to the datablock we want
        var dataPointer = supplyAgentPointer + 104;

        // Dereference and offset to first element
        var dataBlock = new nint(*(long*)dataPointer.ToPointer());

        dataRows = (GrandCompanyDataRow*) dataBlock;
    }

    public GrandCompanyDataRow GetRowForJob(uint classJob)
    {
        const int jobStartIndex = 8;

        return dataRows[classJob - jobStartIndex];
    }
}

[StructLayout(LayoutKind.Explicit, Size = 160)]
public unsafe struct GrandCompanyDataRow
{
    [FieldOffset(0x00)] public Utf8String ItemName;
    [FieldOffset(0x70)] private int IconID;
    [FieldOffset(0x74)] public int ExpReward;
    [FieldOffset(0x78)] public int SealReward;
    [FieldOffset(0x80)] public int NumPossessed;
    [FieldOffset(0x90)] public int NumRequested;
    [FieldOffset(0x9A)] private byte TurnInAvailable;
    [FieldOffset(0x9B)] private byte Bonus;

    public bool IsTurnInAvailable => TurnInAvailable == 0;
    public bool IsBonusReward => Bonus != 0;

    public void Print()
    {
        fixed (Utf8String* stringPointer = &ItemName)
        {
            var seString = MemoryHelper.ReadSeString(stringPointer);

            PluginLog.Debug($"{(IsBonusReward ? "*" : " "), -1} {IconID, -5} {seString.TextValue, -40} {NumRequested, 2} {ExpReward, 8} {SealReward, 5} {NumPossessed, 2}");
        }
    }
}