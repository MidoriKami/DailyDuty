using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class FashionReportModeExtensions {
    extension(FashionReportMode mode) {
        public string Description => mode switch {
            FashionReportMode.All => Strings.FashionReportMode_AllAttemptsUsed,
            FashionReportMode.Single => Strings.FashionReportMode_OneAttemptUsed,
            FashionReportMode.Plus80 => Strings.FashionReportMode_ScoredOver80,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    public static FashionReportMode Parse(string chatType) {
        var result = Enum.GetValues<FashionReportMode>().Where(type => type.Description == chatType).FirstOrDefault();

        return result == default ? FashionReportMode.All : result;
    }
}
