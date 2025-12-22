// using DailyDuty.Classes;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Game.Addon.Events.EventDataTypes;
// using Dalamud.Utility;
// using KamiToolKit.Nodes;
//
// namespace DailyDuty.CustomNodes;
//
// public class TodoTaskNode : TextNode {
// 	public required Module Module { get; init; }
// 	
// 	public ModuleConfig ModuleConfig => Module.GetConfig();
//
// 	public void Refresh() {
// 		IsVisible = Module.IsEnabled && ModuleConfig.TodoEnabled && Module.ModuleStatus is ModuleStatus.Incomplete;
//
// 		// todo fix clickable
// 		// if (Module.HasClickableLink && !IsEventRegistered(AddonEventType.MouseClick)) {
// 		// 	AddEvent(AddonEventType.MouseClick, OnClick);
// 		// }
// 		// else if (!Module.HasClickableLink && IsEventRegistered(AddonEventType.MouseClick)) {
// 		// 	RemoveEvent(AddonEventType.MouseClick, OnClick);
// 		// }
// 		
// 		Tooltip = Module.HasTooltip ? Module.TooltipText : string.Empty;
//
// 		var isTooltipValid = Tooltip is not null && !Tooltip.ToString().IsNullOrEmpty();
// 		// EnableEventFlags = IsVisible && (isTooltipValid || Module.HasClickableLink);
// 	}
//
// 	private void OnClick(AddonEventData data)
// 		=> PayloadController.GetDelegateForPayload(Module.ClickableLinkPayloadId).Invoke(0, null!);
// }
