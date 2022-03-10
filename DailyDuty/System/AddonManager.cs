using System;
using System.Collections.Generic;
using DailyDuty.Addons;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Utilities.Helpers.Addons;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace DailyDuty.System
{
    public class AddonManager : IDisposable
    {
        private readonly List<IAddonModule> overlays = new()
        {
            new DutyFinderAddonModule(),
            new ReconstructionBoxAddonModule(),
            new LotteryWeeklyInputAddonModule(),
            new SatisfactionSupplyRequestAddonModule()
        };

        public static readonly SelectYesNoAddonHelper YesNoAddonHelper = new();

        public AddonManager()
        {
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
