using System;
using DailyDuty.Data.SettingsObjects;

namespace DailyDuty.Interfaces
{
    internal interface IResettable
    {
        public DateTime NextReset { get; set; }

        public bool NeedsResetting()
        {
            return DateTime.UtcNow > NextReset;
        }

        protected DateTime GetNextReset();

        public void DoReset()
        {
            ResetThis();

            NextReset = GetNextReset();

            Service.Configuration.Save();
        }

        protected void ResetThis();

    }
}