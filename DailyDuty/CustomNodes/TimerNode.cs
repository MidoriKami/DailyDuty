using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Newtonsoft.Json;

namespace DailyDuty.CustomNodes;

[JsonObject(MemberSerialization.OptIn)]
public sealed class TimerNode : SimpleComponentNode {
	[JsonProperty] private readonly CastBarProgressBarNode progressBarNode;
	[JsonProperty] private readonly TextNode moduleNameNode;
	[JsonProperty] private readonly TextNineGridNode timeRemainingNode;
	[JsonProperty] private readonly TextNode tooltipNode;

	public TimerNode() {
		progressBarNode = new CastBarProgressBarNode {
			NodeId = 2,
			Progress = 0.30f,
			Size = new Vector2(400.0f, 48.0f),
			BackgroundColor = KnownColor.Black.Vector(),
			BarColor = KnownColor.Aqua.Vector(),
			IsVisible = true,
		};
		System.NativeController.AttachNode(progressBarNode, this);

		moduleNameNode = new TextNode {
			NodeId = 3,
			FontType = FontType.Jupiter,
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 24,
			AlignmentType = AlignmentType.Left,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			IsVisible = true,
		};
		System.NativeController.AttachNode(moduleNameNode, this);

		timeRemainingNode = new TextNineGridNode {
			NodeId = 4,
			TextFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge,
			FontType = FontType.Axis,
			TextOutlineColor = KnownColor.Black.Vector(),
			FontSize = 24,
			Label = "0.00:00:00",
			IsVisible = true,
		};
		System.NativeController.AttachNode(timeRemainingNode, this);

		tooltipNode = new TextNode {
			Size = new Vector2(24.0f, 24.0f),
			NodeId = 5,
			FontSize = 16,
			TextFlags = TextFlags.Edge,
			TextOutlineColor = KnownColor.Black.Vector(),
			AlignmentType = AlignmentType.Center,
			Text = "?",
			Tooltip = "Overlay from DailyDuty plugin",
			EventFlagsSet = true,
			IsVisible = true,
		};
		System.NativeController.AttachNode(tooltipNode, this);
	}

	public float Progress {
		get => progressBarNode.Progress;
		set => progressBarNode.Progress = value;
	}

	protected override void OnSizeChanged() {
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
	
	public SeString ModuleName {
		get => moduleNameNode.Text;
		set => moduleNameNode.Text = value;
	}

	public SeString TimeRemainingText {
		set {
			timeRemainingNode.Label = value;
			timeRemainingNode.X = progressBarNode.Width - timeRemainingNode.Width - 7.5f;
		}
		get => timeRemainingNode.Label;
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

	public override void DrawConfig() {
		base.DrawConfig();
				
		using (var progressBar = ImRaii.TreeNode("Progress Bar")) {
			if (progressBar) {
				progressBarNode.DrawConfig();
			}
		}
				
		using (var moduleName = ImRaii.TreeNode("Module Name")) {
			if (moduleName) {
				moduleNameNode.DrawConfig();
			}
		}
				
		using (var timeRemaining = ImRaii.TreeNode("Time Remaining")) {
			if (timeRemaining) {
				timeRemainingNode.DrawConfig();
			}
		}
				
		using (var tooltip = ImRaii.TreeNode("Tooltip")) {
			if (tooltip) {
				tooltipNode.DrawConfig();
			}
		}
	}
}