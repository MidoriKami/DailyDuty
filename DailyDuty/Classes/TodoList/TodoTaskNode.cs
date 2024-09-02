using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Component.GUI;
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
		NodeFlags |= NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

		if (ModuleConfig.OverrideTextColor) {
			TextColor = ModuleConfig.TodoTextColor;
			TextOutlineColor = ModuleConfig.TodoTextOutline;
		}

		if (Module.HasClickableLink && MouseClick is null) {
			MouseClick = () => PayloadController.GetDelegateForPayload(Module.ClickableLinkPayloadId).Invoke(0, null!);
		}
		else if (!Module.HasClickableLink && MouseClick is not null) {
			MouseClick = null;
		}
		
		Text = ModuleConfig.UseCustomTodoLabel ? ModuleConfig.CustomTodoLabel : Module.ModuleName.GetDescription();

		if (Module.HasTooltip) {
			Tooltip = Module.TooltipText;
		}
		else {
			Tooltip = null;
		}
	}
}