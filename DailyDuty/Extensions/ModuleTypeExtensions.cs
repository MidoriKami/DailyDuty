using System;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ModuleTypeExtensions {
    extension(ModuleType type) {
        public string Description => type switch {
            ModuleType.Daily => "Daily",
            ModuleType.Weekly => "Weekly",
            ModuleType.Special => "Other",
            ModuleType.GeneralFeatures => "Features",
            ModuleType.Hidden => "Debug",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
