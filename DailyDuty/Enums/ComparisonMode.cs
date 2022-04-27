using System;
using DailyDuty.Localization;

namespace DailyDuty.Enums
{
    public enum ComparisonMode
    {
        LessThan,

        EqualTo,

        LessThanOrEqual,
    }

    public static class ComparisonModeExtensions
    {
        public static string GetLabel(this ComparisonMode mode)
        {
            return mode switch
            {
                ComparisonMode.LessThan => Strings.Common.LessThanLabel,
                ComparisonMode.EqualTo => Strings.Common.EqualToLabel,
                ComparisonMode.LessThanOrEqual => Strings.Common.LessThanOrEqualLabel,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
    }
}
