using System.Runtime.InteropServices;

namespace DailyDuty.Data
{
    public enum ButtonState
    {
        // Needs instance completion to become available
        Completable,

        // Can click button to get a stamp right now
        AvailableNow,

        // Already completed, needs re-roll
        Unavailable,

        // Data is state, unknown state
        Unknown
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct WondrousTails
    {
        [FieldOffset(0x06)] public fixed byte Tasks[16];

        [FieldOffset(0x16)] public uint Rewards;

        [FieldOffset(0x1A)] private readonly ushort _stickers;

        public int Stickers => CountSetBits(_stickers);

        [FieldOffset(0x20)] private readonly ushort _secondChance;

        [FieldOffset(0x1E)] public ushort WeeklyKey;

        public int SecondChance => (_secondChance >> 7) & 0b1111;

        [FieldOffset(0x22)] 
        private fixed byte _taskStatus[4];

        public ButtonState TaskStatus(int idx)
            => (ButtonState) ((_taskStatus[idx >> 2] >> ((idx & 0b11) * 2)) & 0b11);

        private static int CountSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }

            return count;
        }
    }
}