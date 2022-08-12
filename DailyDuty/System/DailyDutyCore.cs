using System;
using DailyDuty.Utilities;

namespace DailyDuty.System;

internal class DailyDutyCore : IDisposable
{
    public readonly CommandManager CommandSystem = new();
    public readonly WindowManager WindowManager = new();

    public DailyDutyCore()
    {
        Log.Verbose("Constructing DailyDutyCore");
    }

    public void Dispose()
    {
        CommandSystem.Dispose();
        WindowManager.Dispose();
    }
}