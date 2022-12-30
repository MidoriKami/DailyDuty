using System;
using System.Collections.Generic;

namespace DailyDuty.System;

public static class AddonManager
{
    private static readonly List<IDisposable> LoadedAddons = new();

    public static void AddAddon(IDisposable addon)
    {
        LoadedAddons.Add(addon);
    }
    
    public static void Cleanup()
    {
        foreach (var addon in LoadedAddons)
        {
            addon.Dispose();
        }
    }
}