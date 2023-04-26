using DailyDuty.Models.Attributes;

namespace DailyDuty.Models.Enums;

public enum WindowAnchor
{
	[Label("TopLeft")]
	TopLeft = 0,

	[Label("TopRight")]
	TopRight = 2,

	[Label("BottomLeft")]
	BottomLeft = 1,

	[Label("BottomRight")]
	BottomRight = TopRight | BottomLeft
}