using System;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ModuleTypeExtensions {
    extension(ModuleType type) {
        public string Description => type switch {
            ModuleType.Daily => Strings.Daily,
            ModuleType.Weekly => Strings.Weekly,
            ModuleType.Special => Strings.Other,
            ModuleType.GeneralFeatures => Strings.Features,
            ModuleType.Hidden => Strings.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
