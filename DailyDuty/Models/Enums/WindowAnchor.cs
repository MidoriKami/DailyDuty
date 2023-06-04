using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Enums;

public enum WindowAnchor
{
	[EnumLabel("TopLeft")]
	TopLeft = 0,

	[EnumLabel("TopRight")]
	TopRight = 2,

	[EnumLabel("BottomLeft")]
	BottomLeft = 1,

	[EnumLabel("BottomRight")]
	BottomRight = TopRight | BottomLeft
}