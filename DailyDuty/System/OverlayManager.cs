using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Overlays;

namespace DailyDuty.System
{
    public class OverlayManager : IDisposable
    {
        private readonly List<IOverlay> overlays = new()
        {
            new DutyFinderOverlay()
        };

        public OverlayManager()
        {

        }

        public void Dispose()
        {
            foreach (var overlay in overlays)
            {
                overlay.Dispose();
            }
        }
    }
}
