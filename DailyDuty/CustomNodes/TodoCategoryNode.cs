using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

[JsonObject(MemberSerialization.OptIn)]
public class TodoCategoryNode : NodeBase<AtkResNode> {
	public ModuleType ModuleType { get; private set; }

	[JsonProperty] public TextNode HeaderTextNode { get; private set; }
	[JsonProperty] public ListNode<TodoTaskNode> TaskListNode { get; private set; }

	public TodoCategoryNode(ModuleType type) : base(NodeType.Res) {
		ModuleType = type;
		NodeId = NodeId = 310_000 + (uint) ModuleType;
		Margin = new Spacing(5.0f);

		HeaderTextNode = new TextNode {
			TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
			FontSize = 24,
			Margin = new Spacing(5.0f),
			TextOutlineColor = new Vector4(142, 106, 12, 255) / 255,
			NodeId = NodeId + 500,
			Text = type.GetDescription(),
		};
		
		HeaderTextNode.AddEvent(AddonEventType.MouseClick, System.ConfigurationWindow.UnCollapseOrToggle, true);
		
		System.NativeController.AttachToNode(HeaderTextNode, this, NodePosition.AsFirstChild);

		TaskListNode = new ListNode<TodoTaskNode> {
			NodeId = 310_000 + (uint)ModuleType * 1_000,
			LayoutAnchor = LayoutAnchor.TopLeft,
			LayoutOrientation = LayoutOrientation.Vertical,
			Position = new Vector2(0.0f, HeaderTextNode.Height),
			IsVisible = true,
			BackgroundVisible = false,
		};
		
		System.NativeController.AttachToNode(TaskListNode, this, NodePosition.AsLastChild);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			TaskListNode.Dispose();
			HeaderTextNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public unsafe void LoadNodes(AddonNamePlate* addonNamePlate) {
		TaskListNode.Clear();
		
		foreach (var module in System.ModuleController.GetModules(ModuleType)) {
			var newTaskNode = new TodoTaskNode {
				FontSize = 12,
				Margin = new Spacing(1.0f),  
				TextOutlineColor = new Vector4(10, 105, 146, 255) / 255,
				FontType = FontType.Axis,
				TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Edge,
				NodeId = NodeId + 500 + (uint)module.ModuleName,
				Text = module.ModuleName.GetDescription(),
				IsVisible = module is { IsEnabled: true, ModuleStatus: ModuleStatus.Incomplete },
				Module = module,
			};

			newTaskNode.Load(StyleFileHelper.GetPath($"{module.ModuleName}.style.json"));
			
			module.TodoTaskNode = newTaskNode;

			if (module.HasTooltip) {
				newTaskNode.Tooltip = module.TooltipText;
			}
			
			if (module.HasClickableLink) {
				newTaskNode.AddEvent(AddonEventType.MouseClick, () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!), true);
			}

			if (module is { HasTooltip: true } or { HasClickableLink: true }) {
				newTaskNode.EnableEvents(Service.AddonEventManager, (AtkUnitBase*)addonNamePlate);
			}

			TaskListNode.Add(newTaskNode);
		}
	}

	public override unsafe void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		HeaderTextNode.EnableEvents(eventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		HeaderTextNode.DisableEvents(eventManager);
	}
	
	public bool AnyTasksActive => TaskListNode.Any(nodes => nodes is { Module: { ModuleStatus: ModuleStatus.Incomplete, IsEnabled: true }, ModuleConfig.TodoEnabled: true });

	public void Refresh() {
		IsVisible = AnyTasksActive;

		var headerOffset = HeaderTextNode.IsVisible ? HeaderTextNode.Height : 0.0f;
		
		TaskListNode.Position = new Vector2(0.0f, headerOffset);
		TaskListNode.LayoutAnchor = TaskListNode.LayoutAnchor;
		
		foreach (var node in TaskListNode) {
			node.Refresh();
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