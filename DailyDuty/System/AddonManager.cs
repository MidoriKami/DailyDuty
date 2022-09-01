using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Addons;
using DailyDuty.Addons.Enums;
using DailyDuty.Interfaces;

namespace DailyDuty.System;

internal class AddonManager : IDisposable
{
    private readonly List<IAddon> addons = new()
    {
        new DutyFinderAddon(),
    };
    
    public void Dispose()
    {

    }

    public IAddon this[AddonName name] => addons.First(module => module.Name == name);

    public T GetAddonByType<T>()
    {
        return addons.OfType<T>().First();
    }
}