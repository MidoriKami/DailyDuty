using System;
using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Modules;

namespace DailyDuty.System
{
    public class AddonManager : IDisposable
    {
        private readonly List<IAddonModule> overlays = new()
        {
            new DutyRouletteDutyFinderOverlayAddonModule(),
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
