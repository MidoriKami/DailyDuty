using System;

namespace DailyDuty.Interfaces;

internal interface ITimerComponent
{
    IModule ParentModule { get; }

    TimeSpan GetTimerPeriod();

    DateTime GetNextReset();
}