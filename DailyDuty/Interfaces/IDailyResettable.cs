using System;
using DailyDuty.Utilities;

namespace DailyDuty.Interfaces
{
    internal interface IDailyResettable : IResettable
    {
        DateTime IResettable.GetNextReset()
        {
            return Time.NextDailyReset();
        }
    }
}