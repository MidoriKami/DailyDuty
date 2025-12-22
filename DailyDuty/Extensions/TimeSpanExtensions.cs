using System;

namespace DailyDuty.Extensions;

public static class TimeSpanExtensions {
	extension(TimeSpan timeSpan) {
		public string FormatTimespan(bool hideSeconds = false)
			=> hideSeconds ? 
				   $"{timeSpan.Days:0}.{timeSpan.Hours:00}:{timeSpan.Minutes:00}" : 
				   $"{timeSpan.Days:0}.{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";

		public string FormatTimeSpanShort(bool hideSeconds = false)
			=> hideSeconds ? 
				   $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}" : 
				   $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}
}