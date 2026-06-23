using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class ComparisonModeExtensions {
    extension(ComparisonMode mode) {
        public string Description => mode switch {
            ComparisonMode.Below => Strings.Below,
            ComparisonMode.Equal => Strings.Not_Equal,
            ComparisonMode.Above => Strings.Above,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    public static ComparisonMode Parse(string comparisonMode) {
        var result = Enum.GetValues<ComparisonMode>().Where(type => type.Description == comparisonMode).FirstOrDefault();

        return result == default ? ComparisonMode.Below : result;
    }
}
