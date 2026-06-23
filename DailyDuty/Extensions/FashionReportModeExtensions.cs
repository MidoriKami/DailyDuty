using Resources;
using System;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class FashionReportModeExtensions {
    extension(FashionReportMode mode) {
        public string Description => mode switch {
            FashionReportMode.All => Strings.ResourceManager.GetString("All Attempts Used", Strings.Culture) ?? "All Attempts Used",
            FashionReportMode.Single => Strings.ResourceManager.GetString("One Attempt Used", Strings.Culture) ?? "One Attempt Used",
            FashionReportMode.Plus80 => Strings.ResourceManager.GetString("Scored Over 80", Strings.Culture) ?? "Scored Over 80",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }

    public static FashionReportMode Parse(string chatType) {
        var result = Enum.GetValues<FashionReportMode>().Where(type => type.Description == chatType).FirstOrDefault();

        return result == default ? FashionReportMode.All : result;
    }
}
