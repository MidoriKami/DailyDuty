using Resources;
using System;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ModuleTypeExtensions {
    extension(ModuleType type) {
        public string Description => type switch {
            ModuleType.Daily => Strings.ResourceManager.GetString("Daily", Strings.Culture) ?? "Daily",
            ModuleType.Weekly => Strings.ResourceManager.GetString("Weekly", Strings.Culture) ?? "Weekly",
            ModuleType.Special => Strings.ResourceManager.GetString("Other", Strings.Culture) ?? "Other",
            ModuleType.GeneralFeatures => Strings.ResourceManager.GetString("Features", Strings.Culture) ?? "Features",
            ModuleType.Hidden => Strings.ResourceManager.GetString("Debug", Strings.Culture) ?? "Debug",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
