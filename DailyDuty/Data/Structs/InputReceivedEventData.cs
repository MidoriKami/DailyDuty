using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Utilities;

namespace DailyDuty.Data.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct InputReceivedEventData
    {
        [FieldOffset(0x00)] private fixed byte Data[24];

        [FieldOffset(0x00)] public readonly byte KeyCode;

        [FieldOffset(0x04)] public readonly bool KeyDown;

        public bool KeyUp => !KeyDown;

        public void Print()
        {
            var resultString = "";

            for (int i = 23; i >= 0; i--)
            {
                resultString += $"{Data[i]:x2}";
            }

            Chat.Debug(resultString);
        }
    }
}
