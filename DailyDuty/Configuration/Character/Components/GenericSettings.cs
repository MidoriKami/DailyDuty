using System;
using DailyDuty.Configuration.Common;
using DailyDuty.Modules.Enums;

namespace DailyDuty.Configuration.Character.Components
{
    public class GenericSettings
    {
        public DateTime NextReset = new();
        public Setting<bool> Enabled = new(false);
        public ModuleStatus ModuleStatus = ModuleStatus.Unknown;
    }
}