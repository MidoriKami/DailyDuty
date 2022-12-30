using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;

namespace DailyDuty.System;

internal class AddonManager : IDisposable
{
    private readonly List<IDisposable> addons = new()
    {
        new DutyFinderAddon(),
        new LotteryDailyAddon(),
        new CommendationAddon(),
        new LotteryWeeklyAddon(),
        new GoldSaucerAddon(),
        new WeeklyPuzzleAddon(),
        new AOZContentResultAddon(),
        new GcArmyExpeditionResult(),
    };
    
    public void Dispose()
    {
        foreach (var addon in addons)
        {
            addon.Dispose();
        }
    }

    public T Get<T>()
    {
        return addons.OfType<T>().First();
    }
}