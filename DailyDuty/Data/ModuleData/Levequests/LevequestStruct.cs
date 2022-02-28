using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Data.ModuleData.Levequests
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct LevequestStruct
    {
        [FieldOffset(0x00)] 
        public readonly int AllowancesRemaining;

        [FieldOffset(0xF8)] 
        public readonly byte LevesAccepted;

    }
}
