using System;
using DailyDuty.Addons.Enums;

namespace DailyDuty.Interfaces;

internal interface IAddon : IDisposable
{
    AddonName Name { get; }

    event EventHandler<IntPtr> OnDraw;
    event EventHandler<IntPtr> OnFinalize;
    event EventHandler<IntPtr> OnUpdate;
    event EventHandler<IntPtr> OnRefresh;
}