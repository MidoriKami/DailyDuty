using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
