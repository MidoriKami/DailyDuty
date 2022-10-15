using System;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using Dalamud.Memory;

namespace DailyDuty.DataStructures;

public unsafe class GrandCompanyDataArray
{
    private readonly GrandCompanyDataRow* dataRows;

    public GrandCompanyDataArray(IntPtr supplyAgentPointer)
    {
        // Offset 104 bytes is a pointer to the datablock we want
        var dataPointer = supplyAgentPointer + 104;

        // Dereference and offset to first element
        var dataBlock = new IntPtr(*(long*)dataPointer.ToPointer()) + 32;

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
    [FieldOffset(0x00)] private fixed byte ItemName[0x80];
    [FieldOffset(0x50)] private readonly int IconID;
    [FieldOffset(0x54)] public readonly int ExpReward;
    [FieldOffset(0x58)] public readonly int SealReward;
    [FieldOffset(0x60)] public readonly int NumPossessed;
    [FieldOffset(0x70)] public readonly int NumRequested;
    [FieldOffset(0x7A)] private readonly byte TurnInAvailable;
    [FieldOffset(0x7B)] private readonly byte Bonus;

    public bool IsTurnInAvailable => TurnInAvailable == 0;
    public bool IsBonusReward => Bonus != 0;

    public void Print()
    {
        fixed(byte* str = ItemName)
        {
            var seString = MemoryHelper.ReadSeStringNullTerminated(new IntPtr(str));

            PluginLog.Debug($"Name: {seString.TextValue}\n" +
                            $"Icon: {IconID}\n" +
                            $"ExpReward: {ExpReward}\n" +
                            $"SealReward: {SealReward}\n" +
                            $"NumOwned: {NumPossessed}\n" +
                            $"IsAvailable: {IsTurnInAvailable}\n" +
                            $"NumRequested: {NumRequested}\n" +
                            $"IsBonus: {IsBonusReward}");
        }
    }
}