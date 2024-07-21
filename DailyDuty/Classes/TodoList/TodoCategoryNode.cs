using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.Localization;
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
			FontType = FontType.Axis, 
			TextFlags = TextFlags.AutoAdjustNodeSize,
			MouseClick = () => System.ConfigurationWindow.UnCollapseOrToggle(),
		};
		
		System.NativeController.AttachToNode(headerTextNode, this, NodePosition.AsFirstChild);

		taskListNode = new ListNode<TodoTaskNode> {
			IsVisible = true,
			LayoutOrientation = LayoutOrientation.Vertical,
			NodeID = 310_000 + (uint)moduleType * 1_000,
			Color = KnownColor.White.Vector(),
			Margin = new Spacing(5.0f),
			BackgroundVisible = false,
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
				TextColor = CategoryConfig.ModuleTextColor,
				TextOutlineColor = CategoryConfig.ModuleOutlineColor,
				FontSize = CategoryConfig.ModuleFontSize,
				FontType = FontType.Axis, 
				TextFlags = TextFlags.AutoAdjustNodeSize,
				Text = module.ModuleName.GetDescription(Strings.ResourceManager),
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

	private TextFlags GetHeaderFlags() {
		var flags = TextFlags.AutoAdjustNodeSize;

		if (CategoryConfig.HeaderItalic) flags |= TextFlags.Italic;
		if (CategoryConfig.Edge) flags |= TextFlags.Edge;
		if (CategoryConfig.Glare) flags |= TextFlags.Glare;

		return flags;
	}
    
	public void Refresh() {
		IsVisible = CategoryConfig.Enabled && taskListNode.Any(nodes => nodes is { Module: {ModuleStatus: not ModuleStatus.Complete, IsEnabled: true }});
		
		headerTextNode.TextColor = CategoryConfig.HeaderTextColor;
		headerTextNode.TextOutlineColor = CategoryConfig.HeaderTextOutline;
		headerTextNode.FontSize = CategoryConfig.HeaderFontSize;
		headerTextNode.Text = CategoryConfig.UseCustomLabel ? CategoryConfig.CustomLabel : CategoryConfig.HeaderLabel;
		headerTextNode.TextFlags = GetHeaderFlags();
		headerTextNode.IsVisible = CategoryConfig.ShowHeader;
		
		var headerOffset = CategoryConfig.ShowHeader ? headerTextNode.Height : 0.0f;
		
		taskListNode.Position = new Vector2(0.0f, headerOffset);
		taskListNode.LayoutAnchor = CategoryConfig.LayoutAnchor;
		Margin = new Spacing(CategoryConfig.CategoryMargin.X,
			CategoryConfig.CategoryMargin.Y,
			CategoryConfig.CategoryMargin.Z,
			CategoryConfig.CategoryMargin.W);

		foreach (var node in taskListNode) {
			node.Refresh();
		}
		
		taskListNode.Size = taskListNode.GetMinimumSize();
		if (CategoryConfig.ShowHeader) {
			taskListNode.Width = MathF.Max(taskListNode.Width, headerTextNode.Width);
		}
		
		taskListNode.RecalculateLayout();

		var minSize = taskListNode.GetMinimumSize();

		if (CategoryConfig.ShowHeader) {
			Width = MathF.Max(headerTextNode.LayoutSize.X, minSize.X);
			Height = headerTextNode.LayoutSize.Y + minSize.Y;
		}
		else {
			Size = minSize;
		}
		
		if (CategoryConfig.LayoutAnchor is LayoutAnchor.BottomRight or LayoutAnchor.TopRight) {
			headerTextNode.X = Width - headerTextNode.Width;
		}
	}
}