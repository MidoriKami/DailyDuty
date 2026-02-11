using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Features.TimersOverlay;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Overlay;

namespace DailyDuty.CustomNodes;

public sealed class TimerOverlayNode : OverlayNode {
    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

    private readonly ProgressBarCastNode progressBarNode;
	private readonly TextNode moduleNameNode;
	private readonly TextNineGridNode timeRemainingNode;
	private readonly TextNode tooltipNode;

    public required TimersOverlayConfig TimerTimersOverlayConfig { get; init; }

    public required ModuleBase Module {
        get;
        init {
            field = value;
            moduleNameNode.String = value.Name;
        }
    }

	public TimerOverlayNode() {
        moduleNameNode = new TextNode {
            FontType = FontType.Jupiter,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = 16,
            AlignmentType = AlignmentType.BottomLeft,
            TextFlags = TextFlags.Bold | TextFlags.Edge | TextFlags.Ellipsis,
        };
        moduleNameNode.AttachNode(this);
        
        timeRemainingNode = new TextNineGridNode {
            TextFlags = TextFlags.Bold | TextFlags.Edge,
            FontType = FontType.Axis,
            AlignmentType = AlignmentType.Center,
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = 16,
            String = "0.00:00:00",
        };
        timeRemainingNode.AttachNode(this);
                
        progressBarNode = new ProgressBarCastNode {
            Progress = 0.30f,
            BackgroundColor = KnownColor.Black.Vector(),
            BarColor = KnownColor.Aqua.Vector(),
        };
        progressBarNode.AttachNode(this);

		tooltipNode = new TextNode {
			FontSize = 16,
			TextFlags = TextFlags.Edge,
            TextColor = KnownColor.White.Vector(),
			TextOutlineColor = KnownColor.Black.Vector(),
			AlignmentType = AlignmentType.Center,
			String = "?",
			TextTooltip = "Overlay from DailyDuty plugin",
		};
		tooltipNode.AttachNode(this);

        OnEditComplete = EditComplete;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        
        const float padding = 4.0f;

        moduleNameNode.Size = new Vector2(Width * 3.0f / 5.0f - padding - padding, Height / 2.0f + padding);
        moduleNameNode.Position = new Vector2(0.0f + padding, 0.0f);
        
        timeRemainingNode.Size = new Vector2(Width * 2.0f / 5.0f - padding - padding, Height / 2.0f);
        timeRemainingNode.Position = new Vector2(moduleNameNode.Bounds.Right + padding, 0.0f + padding);

        tooltipNode.Size = new Vector2(16.0f, 16.0f);
        tooltipNode.Position = new Vector2(Width - tooltipNode.Width - padding, Height / 2.0f + tooltipNode.Height / 2.0f);
        
        progressBarNode.Size = new Vector2(Width - tooltipNode.Width - padding, Height / 2.0f);
        progressBarNode.Position = new Vector2(0.0f, Height / 2.0f);
    }
    
    protected override void OnUpdate() {
        var timeRemaining = Module.DataBase.NextReset - DateTime.UtcNow;
        var timerPeriod = Module.GetResetPeriod();
        var percentage = 1.0f - (float) (timeRemaining / timerPeriod);
        percentage = Math.Clamp(percentage, 0.0f, 1.0f);

        progressBarNode.Progress = 1 - percentage;
        progressBarNode.BarColor = TimerTimersOverlayConfig.TimerData[Module.Name].Color;
        timeRemainingNode.String = timeRemaining.FormatTimespan(TimerTimersOverlayConfig.HideTimerSeconds);
        EnableMoving = TimerTimersOverlayConfig.EnableMovingTimers;
        Scale = new Vector2(TimerTimersOverlayConfig.Scale, TimerTimersOverlayConfig.Scale);
        moduleNameNode.IsVisible = TimerTimersOverlayConfig.ShowLabel;
        timeRemainingNode.IsVisible = TimerTimersOverlayConfig.ShowCountdownText;

        var shouldHide = TimerTimersOverlayConfig.HideInDuties && Services.Condition.IsBoundByDuty;
        shouldHide |= TimerTimersOverlayConfig.HideInQuestEvents && Services.Condition.IsInQuestEvent;
        
        IsVisible = !shouldHide;
    }
    
    private void EditComplete(NodeBase nodeBase) {
        TimerTimersOverlayConfig.TimerData[Module.Name].Position = Position;
        TimerTimersOverlayConfig.MarkDirty();
    }
}
