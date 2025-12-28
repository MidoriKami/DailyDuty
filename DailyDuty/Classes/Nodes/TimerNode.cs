using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Extensions;
using DailyDuty.Features.TimersOverlay;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Overlay;

namespace DailyDuty.Classes.Nodes;

public sealed class TimerNode : OverlayNode {
    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

    private readonly ProgressBarCastNode progressBarNode;
	private readonly TextNode moduleNameNode;
	private readonly TextNineGridNode timeRemainingNode;
	private readonly TextNode tooltipNode;

    public required TimersOverlayConfig TimerConfig { get; init; }

    public required ModuleBase Module {
        get;
        init {
            field = value;
            moduleNameNode.String = value.Name;
        }
    }

	public TimerNode() {
        moduleNameNode = new TextNode {
            FontType = FontType.Jupiter,
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
    
    public override void Update() {
        base.Update();

        var timeRemaining = Module.DataBase.NextReset - DateTime.UtcNow;
        var timerPeriod = Module.GetResetPeriod();
        var percentage = 1.0f - (float) (timeRemaining / timerPeriod);

        progressBarNode.Progress = percentage;
        progressBarNode.BarColor = TimerConfig.TimerData[Module.Name].Color;
        timeRemainingNode.String = timeRemaining.FormatTimespan(TimerConfig.HideTimerSeconds);
        EnableMoving = TimerConfig.EnableMovingTimers;
        Scale = new Vector2(TimerConfig.Scale, TimerConfig.Scale);
        moduleNameNode.IsVisible = TimerConfig.ShowLabel;
        timeRemainingNode.IsVisible = TimerConfig.ShowCountdownText;

        var shouldHide = TimerConfig.HideInDuties && Services.Condition.IsBoundByDuty;
        shouldHide |= TimerConfig.HideInQuestEvents && Services.Condition.IsInQuestEvent;
        
        IsVisible = !shouldHide;
    }
    
    private void EditComplete() {
        TimerConfig.TimerData[Module.Name].Position = Position;
        TimerConfig.MarkDirty();
    }
}
