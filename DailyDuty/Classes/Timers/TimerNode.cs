using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Timers;

public unsafe class TimerNode : NodeBase<AtkResNode> {
	private readonly ProgressBarNode progressBarNode;
	private readonly TextNode moduleNameNode;
	private readonly TextNode timeRemainingNode;
	private readonly TextNode tooltipNode;

	private Vector2 actualSize;

	public TimerNode(uint nodeId) : base(NodeType.Res) {
		NodeID = nodeId;
		Color = KnownColor.White.Vector();
		IsVisible = true;
		Position = new Vector2(400.0f, 400.0f);
		
		progressBarNode = new ProgressBarNode(nodeId) {
			NodeID = nodeId + 10000,
			Width = 400.0f,
			Height = 48.0f,
			Progress = 0.30f,
			NodeFlags = NodeFlags.Visible,
		};
		System.NativeController.AttachToNode(progressBarNode, this, NodePosition.AsLastChild);

		moduleNameNode = new TextNode {
			Position = new Vector2(12.0f, -24.0f),
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			NodeID = nodeId + 20000,
			NodeFlags = NodeFlags.Visible,
			FontType = FontType.Jupiter,
			FontSize = 24,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
		};
		System.NativeController.AttachToNode(moduleNameNode, this, NodePosition.AsLastChild);

		timeRemainingNode = new TextNode {
			TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			NodeID = nodeId + 30000,
			NodeFlags = NodeFlags.Visible | NodeFlags.AnchorRight,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			FontType = FontType.Axis,
			FontSize = 24,
			CharSpacing = 0,
			Position = new Vector2(250.0f, -22.0f),
		};
		System.NativeController.AttachToNode(timeRemainingNode, this, NodePosition.AsLastChild);

		tooltipNode = new TextNode {
			NodeID = 250000 + nodeId,
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
		};
		System.NativeController.AttachToNode(tooltipNode, this, NodePosition.AsLastChild);

		Width = 400.0f;
		Height = 48.0f;
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			System.NativeController.DetachFromNode(progressBarNode);
			progressBarNode.Dispose();
			
			System.NativeController.DetachFromNode(moduleNameNode);
			moduleNameNode.Dispose();
			
			System.NativeController.DetachFromNode(timeRemainingNode);
			timeRemainingNode.Dispose();
			
			base.Dispose(disposing);
		}
	}

	public override void EnableEvents(IAddonEventManager eventManager, AtkUnitBase* addon) {
		base.EnableEvents(eventManager, addon);
		
		tooltipNode.EnableEvents(Service.AddonEventManager, addon);
	}

	public override void DisableEvents(IAddonEventManager eventManager) {
		base.DisableEvents(eventManager);
		
		tooltipNode.DisableEvents(eventManager);
	}

	public float Progress {
		get => progressBarNode.Progress;
		set => progressBarNode.Progress = value;
	}

	public new float Width {
		get => actualSize.X;
		set {
			InternalNode->SetWidth((ushort)value);
			progressBarNode.Width = value;
			timeRemainingNode.X = value - timeRemainingNode.Width - 12.0f;
			tooltipNode.Position = new Vector2(progressBarNode.Width, 8.0f);
			actualSize.X = value;
		}
	}

	public new float Height {
		get => actualSize.Y;
		set {
			InternalNode->SetHeight((ushort)value);
			progressBarNode.Height = value;
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

	public SeString ModuleName {
		get => moduleNameNode.Text;
		set => moduleNameNode.Text = value;
	}

	public SeString TimeRemainingText {
		set => timeRemainingNode.Text = value;
	}

	public Vector4 BarColor {
		get => progressBarNode.BarColor;
		set => progressBarNode.BarColor = value;
	}

	public Vector4 BarBackgroundColor {
		get => progressBarNode.BackgroundColor;
		set => progressBarNode.BackgroundColor = value;
	}

	public bool LabelVisible {
		get => moduleNameNode.IsVisible;
		set => moduleNameNode.IsVisible = value;
	}

	public bool TimeVisible {
		get => timeRemainingNode.IsVisible;
		set => timeRemainingNode.IsVisible = value;
	}
}