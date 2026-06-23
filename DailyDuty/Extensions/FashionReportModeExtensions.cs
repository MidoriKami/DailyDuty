using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class FashionReportModeExtensions {
    extension(FashionReportMode mode) {
        public string Description => mode switch {
            FashionReportMode.All => Strings.All_Attempts_Used,
            FashionReportMode.Single => Strings.One_Attempt_Used,
            FashionReportMode.Plus80 => Strings.Scored_Over_80,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    public static FashionReportMode Parse(string chatType) {
        var result = Enum.GetValues<FashionReportMode>().Where(type => type.Description == chatType).FirstOrDefault();

        return result == default ? FashionReportMode.All : result;
    }
}
