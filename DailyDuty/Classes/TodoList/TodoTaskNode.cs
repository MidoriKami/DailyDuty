using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Events;
using KamiLib.Extensions;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.TodoList;

public class TodoTaskNode : TextNode {
	public required Module Module { get; init; }
	
	public ModuleConfig ModuleConfig => Module.GetConfig();
	private CategoryConfig CategoryConfig => System.TodoConfig.CategoryConfigs[(uint) Module.ModuleType];

	public void Refresh() {
		SetStyle(CategoryConfig.ModuleStyle);
		IsVisible = Module.IsEnabled && ModuleConfig.TodoEnabled && Module.ModuleStatus is ModuleStatus.Incomplete;
		SetNodeEventFlags();

		if (ModuleConfig.OverrideTextColor) {
			TextColor = ModuleConfig.TodoTextColor;
			TextOutlineColor = ModuleConfig.TodoTextOutline;
		}

		if (Module.HasClickableLink && !IsEventRegistered(AddonEventType.MouseClick)) {
			AddEvent(AddonEventType.MouseClick, OnClick);
		}
		else if (!Module.HasClickableLink && IsEventRegistered(AddonEventType.MouseClick)) {
			RemoveEvent(AddonEventType.MouseClick, OnClick);
		}
		
		Text = ModuleConfig.UseCustomTodoLabel ? ModuleConfig.CustomTodoLabel : Module.ModuleName.GetDescription();

		if (Module.HasTooltip) {
			Tooltip = Module.TooltipText;
		}
		else {
			Tooltip = null;
		}
	}

	private void OnClick()
		=> PayloadController.GetDelegateForPayload(Module.ClickableLinkPayloadId).Invoke(0, null!);
}