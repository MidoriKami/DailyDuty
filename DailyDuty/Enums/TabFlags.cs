using System;

namespace DailyDuty.Enums
{
    [Flags]
    internal enum TabFlags
    {
        About = 1 << 0,
        Options = 1 << 1,
        Status = 1 << 2,
        Log = 1 << 3,

        All = 0b1111
    }
}
