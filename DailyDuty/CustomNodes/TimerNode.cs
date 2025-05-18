using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

public sealed unsafe class TimerNode : NodeBase<AtkResNode> {
	public readonly ProgressBarNode ProgressBarNode;
	public readonly TextNode ModuleNameNode;
	public readonly TextNode TimeRemainingNode;
	public readonly TextNode TooltipNode;

	private Vector2 actualSize;

	public TimerNode(uint nodeId) : base(NodeType.Res) {
		NodeId = nodeId;
		Color = KnownColor.White.Vector();
		IsVisible = true;
		Position = new Vector2(400.0f, 400.0f);
		
		ProgressBarNode = new ProgressBarNode(nodeId) {
			NodeId = nodeId + 10000,
			Progress = 0.30f,
			Size = new Vector2(400.0f, 48.0f),
			NodeFlags = NodeFlags.Visible,
			BackgroundColor = KnownColor.Black.Vector(),
			BarColor = KnownColor.Aqua.Vector(),
		};
		System.NativeController.AttachToNode(ProgressBarNode, this, NodePosition.AsLastChild);

		ModuleNameNode = new TextNode {
			NodeId = nodeId + 20000,
			Position = new Vector2(12.0f, -24.0f),
			NodeFlags = NodeFlags.Visible,
			FontType = FontType.Jupiter,
			FontSize = 24,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
		};
		System.NativeController.AttachToNode(ModuleNameNode, this, NodePosition.AsLastChild);

		TimeRemainingNode = new TextNode {
			NodeId = nodeId + 30000,
			NodeFlags = NodeFlags.Visible | NodeFlags.AnchorRight,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			FontType = FontType.Axis,
			FontSize = 24,
			CharSpacing = 0,
			Position = new Vector2(0.0f, -22.0f),
			Text = "0.00:00:00",
		};
		System.NativeController.AttachToNode(TimeRemainingNode, this, NodePosition.AsLastChild);

		TooltipNode = new TextNode {
			NodeId = 250000 + nodeId,
			Size = new Vector2(16.0f, 16.0f),
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			IsVisible = true,
			FontSize = 16,
			FontType = FontType.Axis,
			TextFlags = TextFlags.Edge,
			AlignmentType = AlignmentType.BottomRight,
			Text = "?",
			Tooltip = "Overlay from DailyDuty plugin",
			EventFlagsSet = true,
		};
		System.NativeController.AttachToNode(TooltipNode, this, NodePosition.AsLastChild);

		Width = 400.0f;
		Height = 48.0f;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			ProgressBarNode.Dispose();
			ModuleNameNode.Dispose();
			TimeRemainingNode.Dispose();
			TooltipNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		TooltipNode.EnableEvents(Service.AddonEventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		TooltipNode.DisableEvents(eventManager);
	}

	public float Progress {
		get => ProgressBarNode.Progress;
		set => ProgressBarNode.Progress = value;
	}

	public new float Width {
		get => actualSize.X;
		set {
			InternalNode->SetWidth((ushort)value);
			ProgressBarNode.Width = value;
			TimeRemainingNode.X = value - TimeRemainingNode.Width - 12.0f;
			TooltipNode.Position = new Vector2(ProgressBarNode.Width, 8.0f);
			actualSize.X = value;
		}
	}

	public new float Height {
		get => actualSize.Y;
		set {
			InternalNode->SetHeight((ushort)value);
			ProgressBarNode.Height = value;
			actualSize.Y = value;
		}
	}

	public new Vector2 Size {
		get => actualSize;
		set {
			Width = value.X;
			Height = value.Y;
		}
	}

	[JsonIgnore] public SeString ModuleName {
		get => ModuleNameNode.Text;
		set => ModuleNameNode.Text = value;
	}

	[JsonIgnore] public SeString TimeRemainingText {
		set => TimeRemainingNode.Text = value;
		get => TimeRemainingNode.Text;
	}

	public override void DrawConfig() {
		base.DrawConfig();
				
		using (var progressBar = ImRaii.TreeNode("Progress Bar")) {
			if (progressBar) {
				ProgressBarNode.DrawConfig();
			}
		}
				
		using (var moduleName = ImRaii.TreeNode("Module Name")) {
			if (moduleName) {
				ModuleNameNode.DrawConfig();
			}
		}
				
		using (var timeRemaining = ImRaii.TreeNode("Time Remaining")) {
			if (timeRemaining) {
				TimeRemainingNode.DrawConfig();
			}
		}
				
		using (var tooltip = ImRaii.TreeNode("Tooltip")) {
			if (tooltip) {
				TooltipNode.DrawConfig();
			}
		}
	}
}