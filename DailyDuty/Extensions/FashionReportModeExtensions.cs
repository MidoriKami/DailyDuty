using System;
using System.Linq;
using DailyDuty.Enums;

namespace DailyDuty.Extensions;

public static class FashionReportModeExtensions {
    extension(FashionReportMode mode) {
        public string Description => mode switch {
            FashionReportMode.All => "All Attempts Used",
            FashionReportMode.Single => "One Attempt Used",
            FashionReportMode.Plus80 => "Scored Over 80",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
        };
    }
    
    public static FashionReportMode Parse(string chatType) {
        var result = Enum.GetValues<FashionReportMode>().Where(type => type.Description == chatType).FirstOrDefault();
        
        return result == default ? FashionReportMode.All : result;
    }
}
