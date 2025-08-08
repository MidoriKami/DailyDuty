using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Classes;

public unsafe class ContentFinderRouletteListController : ContentFinderListController {
	protected override bool ShouldModifyElement(AtkComponentListItemPopulator.ListItemInfo* listItemInfo)
		=> GetEntryData(listItemInfo).ContentType == ContentsId.ContentsType.Roulette;
}