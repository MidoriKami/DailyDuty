using System.Runtime.InteropServices;
using DailyDuty.Utilities;

namespace DailyDuty.Data.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct MouseClickEventData
    {
        [FieldOffset(0x00)] private fixed byte Data[24];

        public bool RightClick => (Data[6] & 0x01) == 0x01;

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
