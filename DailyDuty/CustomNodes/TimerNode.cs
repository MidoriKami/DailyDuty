using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

[JsonObject(MemberSerialization.OptIn)]
public sealed class TimerNode : SimpleComponentNode {
	private readonly ProgressBarCastNode progressBarNode;
	private readonly TextNode moduleNameNode;
	private readonly TextNineGridNode timeRemainingNode;
	private readonly TextNode tooltipNode;

	public TimerNode() {
		progressBarNode = new ProgressBarCastNode {
			Progress = 0.30f,
			Size = new Vector2(400.0f, 48.0f),
			BackgroundColor = KnownColor.Black.Vector(),
			BarColor = KnownColor.Aqua.Vector(),
		};
		progressBarNode.AttachNode(this);

		moduleNameNode = new TextNode {
			FontType = FontType.Jupiter,
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 24,
			AlignmentType = AlignmentType.Left,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
		};
		moduleNameNode.AttachNode(this);

		timeRemainingNode = new TextNineGridNode {
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			FontType = FontType.Axis,
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 24,
			String = "0.00:00:00",
		};
		timeRemainingNode.AttachNode(this);

		tooltipNode = new TextNode {
			Size = new Vector2(24.0f, 24.0f),
			FontSize = 16,
			TextFlags = TextFlags.Edge,
			TextOutlineColor = KnownColor.Black.Vector(),
			AlignmentType = AlignmentType.Center,
			String = "?",
			Tooltip = "Overlay from DailyDuty plugin",
		};
		tooltipNode.AttachNode(this);
	}

	public float Progress {
		get => progressBarNode.Progress;
		set => progressBarNode.Progress = value;
	}

	protected override void OnSizeChanged() {
		base.OnSizeChanged();
		
		tooltipNode.Size = new Vector2(16.0f, 16.0f);
		progressBarNode.Size = new Vector2(Width - tooltipNode.Width, Height / 2.0f);
		moduleNameNode.Size = new Vector2(Width / 2.0f, Height / 2.0f);
		timeRemainingNode.Size = new Vector2(Width / 2.0f, Height / 2.0f);

		progressBarNode.Position = new Vector2(0.0f, Height / 2.0f);
		moduleNameNode.Position = new Vector2(0.0f, 0.0f);
		timeRemainingNode.Position = new Vector2(Width - tooltipNode.Width - timeRemainingNode.Width, 0.0f);
		tooltipNode.Position = new Vector2(Width - tooltipNode.Width, Height / 2.0f - tooltipNode.Height / 2.0f);
	}

	public AlignmentType ModuleNameAlignment {
		get => moduleNameNode.AlignmentType;
		set => moduleNameNode.AlignmentType = value;
	}
	
	public ReadOnlySeString ModuleName {
		get => moduleNameNode.SeString;
		set => moduleNameNode.SeString = value;
	}

	public ReadOnlySeString TimeRemainingText {
		set {
			timeRemainingNode.SeString = value;
			timeRemainingNode.X = progressBarNode.Width - timeRemainingNode.Width - 7.5f;
		}
		get => timeRemainingNode.SeString;
	}

	public Vector4 BarColor {
		get => progressBarNode.BarColor;
		set => progressBarNode.BarColor = value;
	}

	public Vector4 LabelColor {
		get => moduleNameNode.TextColor;
		set => moduleNameNode.TextColor = value;
	}

	public Vector4 TimerColor {
		get => timeRemainingNode.TextColor;
		set => timeRemainingNode.TextColor = value;
	}

	public bool ShowLabel {
		get => moduleNameNode.IsVisible;
		set => moduleNameNode.IsVisible = value;
	}

	public bool ShowTimer {
		get => timeRemainingNode.IsVisible;
		set => timeRemainingNode.IsVisible = value;
	}
}