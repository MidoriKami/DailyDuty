using DailyDuty.Localization;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.TodoList;

public class TodoTaskNode : TextNode {
	public required BaseModule Module { get; init; }
	
	private ModuleConfigBase ModuleConfig => Module.GetConfig();
	private CategoryConfig CategoryConfig => System.TodoConfig.CategoryConfigs[(uint) Module.ModuleType];

	public void Refresh() {
		IsVisible = Module.IsEnabled && ModuleConfig.TodoEnabled && Module.ModuleStatus is ModuleStatus.Incomplete;

		TextFlags = GetModuleFlags();
		FontSize = CategoryConfig.ModuleFontSize;
		
		Margin = new Spacing(CategoryConfig.ModuleMargin.X,
			CategoryConfig.ModuleMargin.Y,
			CategoryConfig.ModuleMargin.Z,
			CategoryConfig.ModuleMargin.W);
		
		if (ModuleConfig.OverrideTextColor) {
			TextColor = ModuleConfig.TodoTextColor;
			TextOutlineColor = ModuleConfig.TodoTextOutline;
		}
		else {
			TextColor = CategoryConfig.ModuleTextColor;
			TextOutlineColor = CategoryConfig.ModuleOutlineColor;
		}
		
		Text = ModuleConfig.UseCustomTodoLabel ? ModuleConfig.CustomTodoLabel : Module.ModuleName.GetDescription(Strings.ResourceManager);
	}
	
	private TextFlags GetModuleFlags() {
		var flags = TextFlags.AutoAdjustNodeSize;

		if (CategoryConfig.ModuleItalic) flags |= TextFlags.Italic;
		if (CategoryConfig.Edge) flags |= TextFlags.Edge;
		if (CategoryConfig.Glare) flags |= TextFlags.Glare;

		return flags;
	}
}