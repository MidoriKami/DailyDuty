using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

[JsonObject(MemberSerialization.OptIn)]
public class TodoCategoryNode : SimpleComponentNode {
	public ModuleType ModuleType { get; private set; }

	[JsonProperty] public TextNode HeaderTextNode { get; private set; }
	[JsonProperty] public ListBoxNode TaskListNode { get; private set; }

	public readonly List<TodoTaskNode?> TaskNodes = [];

	public TodoCategoryNode(ModuleType type) {
		ModuleType = type;
		Margin = new Spacing(5.0f);

		HeaderTextNode = new TextNode {
			NodeId = 2,
			TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
			FontSize = 24,
			Margin = new Spacing(5.0f),
			TextOutlineColor = new Vector4(142, 106, 12, 255) / 255,
			Text = type.GetDescription(),
		};
		
		HeaderTextNode.AddEvent(AddonEventType.MouseClick, _ => System.ConfigurationWindow.UnCollapseOrToggle(), true);
		
		System.NativeController.AttachNode(HeaderTextNode, this);

		TaskListNode = new ListBoxNode {
			NodeId = 3,
			LayoutAnchor = LayoutAnchor.TopLeft,
			LayoutOrientation = LayoutOrientation.Vertical,
			Position = new Vector2(0.0f, HeaderTextNode.Height),
			IsVisible = true,
			ShowBackground = false,
		};
		
		System.NativeController.AttachNode(TaskListNode, this);
	}

	public void LoadNodes() {
		TaskListNode.Clear();
		TaskNodes.Clear();
		
		foreach (var module in System.ModuleController.GetModules(ModuleType)) {
			var newTaskNode = new TodoTaskNode {
				FontSize = 12,
				Margin = new Spacing(1.0f),  
				TextOutlineColor = ColorHelper.GetColor(53),
				FontType = FontType.Axis,
				TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge,
				Text = module.ModuleName.GetDescription(),
				IsVisible = module is { IsEnabled: true, ModuleStatus: ModuleStatus.Incomplete },
				Module = module,
			};
			
			TaskNodes.Add(newTaskNode);

			newTaskNode.Load(StyleFileHelper.GetPath($"{module.ModuleName}.style.json"));
			
			module.TodoTaskNode = newTaskNode;

			if (module.HasTooltip) {
				newTaskNode.Tooltip = module.TooltipText;
			}
			
			if (module.HasClickableLink) {
				newTaskNode.AddEvent(AddonEventType.MouseClick, _ => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!), true);
			}

			if (module is { HasTooltip: true } or { HasClickableLink: true }) {
				newTaskNode.SetEventFlags();
			}

			TaskListNode.AddNode(newTaskNode);
		}
	}
	
	public bool AnyTasksActive => TaskNodes.Any(nodes => nodes is { Module: { ModuleStatus: ModuleStatus.Incomplete, IsEnabled: true }, ModuleConfig.TodoEnabled: true });

	public void Refresh() {
		IsVisible = AnyTasksActive;

		var headerOffset = HeaderTextNode.IsVisible ? HeaderTextNode.Height : 0.0f;

		if (TaskListNode.LayoutAnchor is LayoutAnchor.TopLeft or LayoutAnchor.BottomLeft) {
			TaskListNode.Position = new Vector2(0.0f, headerOffset);
		}
		else if (TaskListNode.LayoutAnchor is LayoutAnchor.TopRight or LayoutAnchor.BottomRight) {
			TaskListNode.Position = new Vector2(Width - TaskListNode.Width, headerOffset);
		}
		
		foreach (var node in TaskNodes) {
			node?.Refresh();
		}
		
		TaskListNode.Size = TaskListNode.GetMinimumSize();
		if (HeaderTextNode.IsVisible) {
			TaskListNode.Width = MathF.Max(TaskListNode.Width, HeaderTextNode.Width);
		}
		
		TaskListNode.RecalculateLayout();
		
		var minSize = TaskListNode.GetMinimumSize();
		
		if (HeaderTextNode.IsVisible) {
			Width = MathF.Max(HeaderTextNode.LayoutSize.X, minSize.X);
			Height = HeaderTextNode.LayoutSize.Y + minSize.Y;
		}
		else {
			Size = minSize;
		}
		
		if (TaskListNode.LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
			HeaderTextNode.X = Width - HeaderTextNode.Width;
		}
		else {
			HeaderTextNode.X = 0.0f;
		}
	}

	public override void DrawConfig() {
		base.DrawConfig();
				
		using (var header = ImRaii.TreeNode("Header Text Node")) {
			if (header) {
				HeaderTextNode.DrawConfig();
			}
		}
				
		using (var listNode = ImRaii.TreeNode("List Node")) {
			if (listNode) {
				TaskListNode.DrawConfig();
			}
		}
	}
}