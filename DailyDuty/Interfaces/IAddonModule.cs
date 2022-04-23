using System;
using DailyDuty.Enums;

namespace DailyDuty.Interfaces
{
    internal interface IAddonModule : IDisposable
    {
        public AddonName AddonName { get; }
    }
}
