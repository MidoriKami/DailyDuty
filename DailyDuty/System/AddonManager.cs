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
            new LotteryWeeklyInputAddonModule()
        };

        public static SelectYesNoAddonHelper YesNoAddonHelper;

        public AddonManager()
        {
            YesNoAddonHelper = new();

            //Chat.Debug("Addon Manager Loading.");

            //foreach (var addon in overlays)
            //{
            //    Chat.Debug("Loading: " + addon.AddonName);
            //}

            //Chat.Debug("Addon Manager Loading Complete.");
        }

        public void Dispose()
        {
            foreach (var overlay in overlays)
            {
                overlay.Dispose();
            }

            YesNoAddonHelper.Dispose();
        }
    }
}
