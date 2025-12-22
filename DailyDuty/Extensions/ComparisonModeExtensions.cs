using System;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ComparisonModeExtensions {
    extension(ComparisonMode mode) {
        public string Description => mode switch {
            ComparisonMode.LessThan => "Less than",
            ComparisonMode.EqualTo => "Equal to",
            ComparisonMode.LessThanOrEqual => "Less than or equal to",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }
}
