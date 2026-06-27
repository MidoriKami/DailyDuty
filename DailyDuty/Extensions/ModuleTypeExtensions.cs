using System;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ModuleTypeExtensions {
    extension(ModuleType type) {
        public string Description => type switch {
            ModuleType.Daily => Strings.ModuleTypes_Daily,
            ModuleType.Weekly => Strings.ModuleTypes_Weekly,
            ModuleType.Special => Strings.ModuleTypes_Other,
            ModuleType.GeneralFeatures => Strings.ModuleTypes_Features,
            ModuleType.Hidden => Strings.ModuleTypes_Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
