using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Models;
using DailyDuty.Modules;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.TodoList;

public class TodoCategoryNode : NodeBase<AtkResNode> {
	private readonly ModuleType moduleType;
	private CategoryConfig CategoryConfig => System.TodoConfig.CategoryConfigs[(uint)moduleType];

	private readonly TextNode headerTextNode;
	private readonly ListNode<TodoTaskNode> taskListNode;

	public TodoCategoryNode(ModuleType type) : base(NodeType.Res) {
		moduleType = type;
		NodeID = NodeID = 310_000 + (uint) moduleType;
		Margin = new Spacing(5.0f);

		headerTextNode = new TextNode {
			NodeID = NodeID + 500,
			MouseClick = () => System.ConfigurationWindow.UnCollapseOrToggle(),
		};
		
		System.NativeController.AttachToNode(headerTextNode, this, NodePosition.AsFirstChild);

		taskListNode = new ListNode<TodoTaskNode> {
			NodeID = 310_000 + (uint)moduleType * 1_000,
		};
		
		System.NativeController.AttachToNode(taskListNode, this, NodePosition.AsLastChild);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			System.NativeController.DetachFromNode(taskListNode);
			System.NativeController.DetachFromNode(headerTextNode);
			
			taskListNode.Dispose();
			headerTextNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public unsafe void LoadNodes(AddonNamePlate* addonNamePlate) {
		taskListNode.Clear();
		
		foreach (var module in System.ModuleController.GetModules(CategoryConfig.ModuleType)) {
			var newTaskNode = new TodoTaskNode {
				NodeID = NodeID + 500 + (uint)module.ModuleName,
				Text = module.ModuleName.GetDescription(),
				IsVisible = module is { IsEnabled: true, ModuleStatus: ModuleStatus.Incomplete },
				Module = module,
			};

			if (module.HasTooltip) {
				newTaskNode.Tooltip = module.TooltipText;
			}
			
			if (module.HasClickableLink) {
				newTaskNode.MouseClick = () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!);
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

	public void Refresh() {
		taskListNode.SetStyle(CategoryConfig.ListNodeStyle);
		IsVisible = CategoryConfig.Enabled && taskListNode.Any(nodes => nodes is { Module: { ModuleStatus: ModuleStatus.Incomplete, IsEnabled: true }});

		headerTextNode.SetStyle(CategoryConfig.HeaderStyle);
		headerTextNode.Text = CategoryConfig.UseCustomLabel ? CategoryConfig.CustomLabel : CategoryConfig.HeaderLabel;
		headerTextNode.NodeFlags |= NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

		var headerOffset = CategoryConfig.HeaderStyle.IsVisible ? headerTextNode.Height : 0.0f;
		
		taskListNode.Position = new Vector2(0.0f, headerOffset);
		taskListNode.LayoutAnchor = CategoryConfig.ListNodeStyle.LayoutAnchor;
		
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
		
		if (CategoryConfig.ListNodeStyle.LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
			headerTextNode.X = Width - headerTextNode.Width;
		}
	}
}