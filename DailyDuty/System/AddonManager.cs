using System;
using System.Collections.Generic;
using DailyDuty.Features;

namespace DailyDuty.System
{
    public class AddonManager : IDisposable
    {
        private readonly List<IDisposable> overlays = new()
        {
            new DutyRouletteDutyFinderOverlayAddonModule(),
            new WondrousTailsDutyFinderOverlayAddonModule(),
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
