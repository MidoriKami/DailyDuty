using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;

namespace DailyDuty.Classes;

public abstract unsafe class ContentFinderListController() : ListController<AddonContentsFinder>("ContentsFinder") {
	protected override delegate* unmanaged<AtkUnitBase*, AtkComponentListItemPopulator.ListItemInfo*, AtkResNode**, void> GetPopulateDelegate(AddonContentsFinder* addon)
		=> addon->DutyList->GetItemRendererByNodeId(6)->Populator.Populate;

	protected ContentsId GetEntryData(AtkComponentListItemPopulator.ListItemInfo* listItemInfo) {
		var contentId = listItemInfo->ListItem->UIntValues[1];
		var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
		return contentEntry.Value->Id;
	}
}