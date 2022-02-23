using System;
using DailyDuty.Utilities;

namespace DailyDuty.Interfaces
{
    internal interface IWeeklyResettable : IResettable
    {
        DateTime IResettable.GetNextReset()
        {
            return Time.NextWeeklyReset();
        }
    }
}