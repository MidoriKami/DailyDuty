using DailyDuty.Classes;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Events;
using Dalamud.Utility;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class TodoTaskNode : TextNode {
	public required Module Module { get; init; }
	
	public ModuleConfig ModuleConfig => Module.GetConfig();

	public void Refresh() {
		IsVisible = Module.IsEnabled && ModuleConfig.TodoEnabled && Module.ModuleStatus is ModuleStatus.Incomplete;

		if (Module.HasClickableLink && !IsEventRegistered(AddonEventType.MouseClick)) {
			AddEvent(AddonEventType.MouseClick, OnClick);
		}
		else if (!Module.HasClickableLink && IsEventRegistered(AddonEventType.MouseClick)) {
			RemoveEvent(AddonEventType.MouseClick, OnClick);
		}
		
		Tooltip = Module.HasTooltip ? Module.TooltipText : string.Empty;

		EnableEventFlags = IsVisible && (!Tooltip.ToString().IsNullOrEmpty() || Module.HasClickableLink);
	}

	private void OnClick()
		=> PayloadController.GetDelegateForPayload(Module.ClickableLinkPayloadId).Invoke(0, null!);
}