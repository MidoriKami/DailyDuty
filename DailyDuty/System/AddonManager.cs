using System;
using System.Collections.Generic;
using DailyDuty.Addons;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Addons;

namespace DailyDuty.System
{
    public class AddonManager : IDisposable
    {
        private readonly List<IAddonModule> overlays = new()
        {
            new DutyFinderAddonModule(),
        };

        public void Dispose()
        {
            foreach (var overlay in overlays)
            {
                overlay.Dispose();
            }
        }
    }
}
