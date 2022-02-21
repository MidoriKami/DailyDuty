using System;
using DailyDuty.Data.Enums;

namespace DailyDuty.Interfaces
{
    internal interface IAddonModule : IDisposable
    {
        public AddonName AddonName { get; }
    }
}
