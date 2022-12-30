using System;
using System.Numerics;
using DailyDuty.Localization;
using KamiLib.Utilities;

namespace DailyDuty.DataModels;

public enum ModuleStatus
{
    Unknown,
    Incomplete,
    Unavailable,
    Complete,
    Suppressed,
}

public static class ModuleStatusExtensions
{
    public static string GetTranslatedString(this ModuleStatus value)
    {
        return value switch
        {
            ModuleStatus.Unknown => Strings.Common.Unknown,
            ModuleStatus.Incomplete => Strings.Common.Incomplete,
            ModuleStatus.Unavailable => Strings.Common.Unavailable,
            ModuleStatus.Complete => Strings.Common.Complete,
            ModuleStatus.Suppressed => Strings.Common.Suppressed,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static Vector4 GetStatusColor(this ModuleStatus value)
    {
        return value switch
        {
            ModuleStatus.Unknown => Colors.Grey,
            ModuleStatus.Incomplete => Colors.Red,
            ModuleStatus.Unavailable => Colors.Orange,
            ModuleStatus.Complete => Colors.Green,
            ModuleStatus.Suppressed => Colors.Purple,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}