using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Classes;

public unsafe class ContentFinderDutyListController : ContentFinderListController {
	protected override bool ShouldModifyElement(AtkComponentListItemPopulator.ListItemInfo* listItemInfo)
		=> GetEntryData(listItemInfo).ContentType is ContentsId.ContentsType.Regular;
}