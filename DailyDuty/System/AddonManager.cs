using System;
using System.Collections.Generic;
using DailyDuty.Addons;
using DailyDuty.Interfaces;

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
