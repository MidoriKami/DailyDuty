using System;

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
            var storedNextReset = GetNextReset();

            if (storedNextReset != DateTime.MinValue)
            {
                ResetThis();

                NextReset = storedNextReset;

                Service.Configuration.Save();
            }
        }

        protected void ResetThis();

    }
}