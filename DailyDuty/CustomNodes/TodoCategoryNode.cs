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

	[JsonProperty] private readonly TextNode headerTextNode;
	[JsonProperty] private readonly ListNode<TodoTaskNode> taskListNode;

	public TodoCategoryNode(ModuleType type) : base(NodeType.Res) {
		ModuleType = type;
		NodeId = NodeId = 310_000 + (uint) ModuleType;
		Margin = new Spacing(5.0f);

		headerTextNode = new TextNode {
			TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
			FontSize = 24,
			Margin = new Spacing(5.0f),
			TextOutlineColor = new Vector4(142, 106, 12, 255) / 255,
			NodeId = NodeId + 500,
			Text = type.GetDescription(),
			EventFlagsSet = true,
		};
		
		headerTextNode.AddEvent(AddonEventType.MouseClick, System.ConfigurationWindow.UnCollapseOrToggle);
		
		System.NativeController.AttachToNode(headerTextNode, this, NodePosition.AsFirstChild);

		taskListNode = new ListNode<TodoTaskNode> {
			NodeId = 310_000 + (uint)ModuleType * 1_000,
			LayoutAnchor = LayoutAnchor.TopLeft,
			LayoutOrientation = LayoutOrientation.Vertical,
			Position = new Vector2(0.0f, headerTextNode.Height),
			IsVisible = true,
			BackgroundVisible = false,
		};
		
		System.NativeController.AttachToNode(taskListNode, this, NodePosition.AsLastChild);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			taskListNode.Dispose();
			headerTextNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public unsafe void LoadNodes(AddonNamePlate* addonNamePlate) {
		taskListNode.Clear();
		
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
				newTaskNode.AddEvent(AddonEventType.MouseClick, () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!));
			}

			if (module is { HasTooltip: true } or { HasClickableLink: true }) {
				newTaskNode.EnableEvents(Service.AddonEventManager, (AtkUnitBase*)addonNamePlate);
			}

			taskListNode.Add(newTaskNode);
		}
	}

	public override unsafe void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		headerTextNode.EnableEvents(eventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		headerTextNode.DisableEvents(eventManager);
	}
	
	public bool AnyTasksActive => taskListNode.Any(nodes => nodes is { Module: { ModuleStatus: ModuleStatus.Incomplete, IsEnabled: true }, ModuleConfig.TodoEnabled: true });

	public void Refresh() {
		IsVisible = AnyTasksActive;

		var headerOffset = headerTextNode.IsVisible ? headerTextNode.Height : 0.0f;
		
		taskListNode.Position = new Vector2(0.0f, headerOffset);
		taskListNode.LayoutAnchor = taskListNode.LayoutAnchor;
		
		foreach (var node in taskListNode) {
			node.Refresh();
		}
		
		taskListNode.Size = taskListNode.GetMinimumSize();
		if (headerTextNode.IsVisible) {
			taskListNode.Width = MathF.Max(taskListNode.Width, headerTextNode.Width);
		}
		
		taskListNode.RecalculateLayout();
		
		var minSize = taskListNode.GetMinimumSize();
		
		if (headerTextNode.IsVisible) {
			Width = MathF.Max(headerTextNode.LayoutSize.X, minSize.X);
			Height = headerTextNode.LayoutSize.Y + minSize.Y;
		}
		else {
			Size = minSize;
		}
		
		if (taskListNode.LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
			headerTextNode.X = Width - headerTextNode.Width;
		}
	}

	public override void DrawConfig() {
		base.DrawConfig();
				
		using (var header = ImRaii.TreeNode("Header Text Node")) {
			if (header) {
				headerTextNode.DrawConfig();
			}
		}
				
		using (var listNode = ImRaii.TreeNode("List Node")) {
			if (listNode) {
				taskListNode.DrawConfig();
			}
		}
	}
}