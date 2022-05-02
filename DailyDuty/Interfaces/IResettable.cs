using System;
using DailyDuty.Data.Components;

namespace DailyDuty.Interfaces
{
    internal interface IResettable
    {
        public GenericSettings GenericSettings { get; }

        public bool NeedsResetting()
        {
            return DateTime.UtcNow > GenericSettings.NextReset;
        }

        protected DateTime GetNextReset();

        public void DoReset()
        {
            var storedNextReset = GetNextReset();

            if (storedNextReset != DateTime.MinValue && Service.LoggedIn)
            {
                ResetThis();

                GenericSettings.NextReset = storedNextReset;

                Service.CharacterConfiguration.Save();
            }
        }

        protected void ResetThis();

    }
}