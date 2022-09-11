using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyDuty.Utilities;

internal static unsafe class RETools
{
    public static void PrintArray(void* a2, int rows = 8, int columns = 16)
    {
        List<List<byte>> bytes = new();

        for (var i = 0; i < rows; ++i)
        {
            bytes.Add(new List<byte>());

            for (var j = 0; j < columns; ++j)
            {
                bytes[i].Add(((byte*) a2)[i * 8 + j]);
            }
        }

        bytes.Reverse();

        foreach (var row in bytes)
        {
            row.Reverse();
            var str = string.Join(" ", row.Select(num => string.Format($"{num:X2}")));

            Log.Verbose(str);
        }
    }

    public static byte* GetOffsetAddress(IntPtr pointer, int offset)
    {
        return ((byte*) pointer) + offset;
    }
}