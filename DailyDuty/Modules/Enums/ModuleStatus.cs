using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Modules.Enums;

public enum ModuleStatus
{
    Unknown,
    Incomplete,
    Unavailable,
    Complete
}

public static class ModuleStatusExtensions
{
    public static string GetLocalizedString(this ModuleStatus value)
    {
        return value switch
        {
            ModuleStatus.Unknown => Strings.Common.Unknown,
            ModuleStatus.Incomplete => Strings.Common.Incomplete,
            ModuleStatus.Unavailable => Strings.Common.Unavailable,
            ModuleStatus.Complete => Strings.Common.Complete,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}