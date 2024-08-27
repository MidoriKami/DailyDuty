using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.NodeStyles;

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
			Progress = 0.30f,
		};
		System.NativeController.AttachToNode(progressBarNode, this, NodePosition.AsLastChild);

		moduleNameNode = new TextNode {
			NodeID = nodeId + 20000,
		};
		System.NativeController.AttachToNode(moduleNameNode, this, NodePosition.AsLastChild);

		timeRemainingNode = new TextNode {
			NodeID = nodeId + 30000,
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

	public void SetStyle(TimerNodeStyle style) {
		SetStyle(style as NodeBaseStyle);
		tooltipNode.NodeFlags |= NodeFlags.EmitsEvents | NodeFlags.HasCollision | NodeFlags.RespondToMouse;

		progressBarNode.SetStyle(style.ProgressBarNodeStyle);
		moduleNameNode.SetStyle(style.ModuleNameStyle);
		timeRemainingNode.SetStyle(style.TimerTextStyle);
	}
}