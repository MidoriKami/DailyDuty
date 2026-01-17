using DailyDuty.Classes;
using DailyDuty.Enums;

namespace DailyDuty.Features.FashionReport;

public class FashionReportConfig : ConfigBase {
	public FashionReportMode CompletionMode = FashionReportMode.Single;
}
