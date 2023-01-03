using System;
using DailyDuty.Localization;

namespace DailyDuty.DataModels;

public enum ComparisonMode
{
    LessThan,
    EqualTo,
    LessThanOrEqual,
}

public static class ComparisonModeExtensions
{
    public static string GetTranslatedString(this ComparisonMode mode)
    {
        return mode switch
        {
            ComparisonMode.LessThan => Strings.Common_LessThan,
            ComparisonMode.EqualTo => Strings.Common_Equal,
            ComparisonMode.LessThanOrEqual => Strings.Common_LessThanEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}