using System;

namespace DailyDuty.Extensions;

public static class DateTimeExtensions {
    extension(DateTime date) {
        public string GetDisplayString()
            => $"{date.ToLongDateString()} {date.ToLongTimeString()}";
    }
}
