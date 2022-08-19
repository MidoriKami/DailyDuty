using System;
using DailyDuty.System.Localization;

namespace DailyDuty.Modules.Enums;

public enum CompletionType
{
    Daily,
    Weekly
}

public static class CompletionTypeExtensions
{
    public static string GetLocalizedString(this CompletionType value)
    {
        return value switch
        {
            CompletionType.Daily => Strings.Common.Daily,
            CompletionType.Weekly => Strings.Common.Weekly,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}