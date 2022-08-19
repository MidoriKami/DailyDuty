using System;
using DailyDuty.Configuration.Common;
using DailyDuty.Modules.Enums;

namespace DailyDuty.Configuration.Character.Components
{
    public class GenericSettings
    {
        public DateTime NextReset = new();
        public ModuleStatus ModuleStatus = ModuleStatus.Unknown;
        
        public readonly Setting<bool> Enabled = new(false);
        public readonly Setting<bool> NotifyOnLogin = new(true);
        public readonly Setting<bool> NotifyOnZoneChange = new(true);
    }
}