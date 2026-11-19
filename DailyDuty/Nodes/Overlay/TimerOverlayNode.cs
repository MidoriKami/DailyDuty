using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;

namespace DailyDuty.Nodes.Overlay;

public sealed class TimerOverlayNode : OverlayNode {

    public override OverlayLayer OverlayLayer => OverlayLayer.BehindUserInterface;

    private readonly ProgressBarCastNode progressBarNode;
    private readonly TextNode moduleNameNode;
    private readonly TextNineGridNode timeRemainingNode;
    private readonly TextNode tooltipNode;

    public IModule Module { get; }
    public TimerOverlayConfig Config { get; }
    public TimerOverlayTimerData? TimerData { get; }

    private TimerCountDirection countDirection;
    private Vector2? pendingPosition;

    public void SetInitialPosition(Vector2 position) {
        pendingPosition = position;
    }

    public TimerOverlayNode(IModule module, TimerOverlayConfig config, TimerOverlayTimerData? timerData) {
        Module = module;
        Config = config;
        TimerData = timerData;
        countDirection = TimerData?.CountDirection ?? TimerCountDirection.CountDown;

        var textColor = TimerData?.TextColor ?? KnownColor.White.Vector();
        var fontSize = (uint)(TimerData?.FontSize ?? 16);

        moduleNameNode = new TextNode {
            FontType = FontType.Jupiter,
            TextColor = textColor,
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = fontSize,
            AlignmentType = AlignmentType.BottomLeft,
            TextFlags = TextFlags.Bold | TextFlags.Edge | TextFlags.Ellipsis,
        };
        moduleNameNode.AttachNode(this);

        timeRemainingNode = new TextNineGridNode {
            TextFlags = TextFlags.Bold | TextFlags.Edge,
            FontType = FontType.Axis,
            AlignmentType = AlignmentType.Center,
            TextColor = textColor,
            TextOutlineColor = KnownColor.Black.Vector(),
            FontSize = fontSize,
            String = "0.00:00:00",
        };
        timeRemainingNode.AttachNode(this);

        progressBarNode = new ProgressBarCastNode {
            Progress = 0.0f,
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
            TextTooltip = Strings.TimerOverlayNode_OverlayTooltip,
        };
        tooltipNode.AttachNode(this);

        OnEditComplete = _ => SavePosition();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        moduleNameNode.Size = new Vector2(Width * 3.0f / 5.0f - 4.0f - 4.0f, Height / 2.0f + 4.0f);
        moduleNameNode.Position = new Vector2(0.0f + 4.0f, 0.0f);

        timeRemainingNode.Size = new Vector2(Width * 2.0f / 5.0f - 4.0f - 4.0f, Height / 2.0f);
        timeRemainingNode.Position = new Vector2(moduleNameNode.Bounds.Right + 4.0f, 0.0f + 4.0f);

        tooltipNode.Size = new Vector2(16.0f, 16.0f);
        tooltipNode.Position = new Vector2(Width - tooltipNode.Width - 4.0f, Height / 2.0f + tooltipNode.Height / 2.0f);

        progressBarNode.Size = new Vector2(Width - tooltipNode.Width - 4.0f, Height / 2.0f);
        progressBarNode.Position = new Vector2(0.0f, Height / 2.0f);
    }

    protected override void OnUpdate() {
        // Apply pending initial position on the first frame after attachment
        if (pendingPosition.HasValue) {
            Position = pendingPosition.Value;
            pendingPosition = null;
        }

        // Check for dirty config and apply all changes
        if (TimerData is { IsDirty: true }) {
            TimerData.IsDirty = false;
            countDirection = TimerData.CountDirection;
            moduleNameNode.String = TimerData.Label ?? Module.DisplayName;
            var textColor = TimerData.TextColor;
            moduleNameNode.TextColor = textColor;
            timeRemainingNode.TextColor = textColor;
            var fontSize = (uint)TimerData.FontSize;
            moduleNameNode.FontSize = fontSize;
            timeRemainingNode.FontSize = fontSize;
            progressBarNode.BarColor = TimerData.Color;

            var nodeWidth = TimerData.Width ?? 300;
            var nodeHeight = TimerData.Height ?? 64;
            if (Width != nodeWidth || Height != nodeHeight) {
                Size = new Vector2(nodeWidth, nodeHeight);
            }
        }

        var scale = TimerData?.Scale ?? 1.0f;
        Scale = new Vector2(scale, scale);
        EnableMoving = TimerData?.EnableMoving ?? true;

        // Apply per-timer dimensions
        var curWidth = TimerData?.Width ?? 300;
        var curHeight = TimerData?.Height ?? 64;
        if (Width != curWidth || Height != curHeight) {
            Size = new Vector2(curWidth, curHeight);
            return;
        }

        // Update label from per-timer config
        moduleNameNode.String = TimerData?.Label ?? Module.DisplayName;
        moduleNameNode.IsVisible = TimerData?.ShowLabel ?? true;
        timeRemainingNode.IsVisible = TimerData?.ShowCountdown ?? true;

        // Compute time remaining and progress
        var nextReset = Module.GetNextResetTime();
        var windowEnd = Module.GetWindowEndTime() ?? nextReset;

        // If reset time is not yet available (e.g., TreasureMap not yet gathered), hide the timer
        if (windowEnd == DateTime.MaxValue) {
            IsVisible = false;
            return;
        }

        var timeRemaining = windowEnd.ToUniversalTime() - DateTime.UtcNow;
        var timerPeriod = Module.ResetPeriod;

        // Hide when completed
        if ((TimerData?.HideWhenCompleted ?? false) && timeRemaining.TotalSeconds <= 0) {
            IsVisible = false;
            return;
        }

        if (timeRemaining.TotalSeconds <= 0) {
            progressBarNode.Progress = countDirection == TimerCountDirection.CountUp ? 0.0f : 1.0f;
            timeRemainingNode.String = "0.00:00:00";
        }
        else {
            var percentage = 1.0f - (float)(timeRemaining.TotalSeconds / timerPeriod.TotalSeconds);
            percentage = Math.Clamp(percentage, 0.0f, 1.0f);
            progressBarNode.Progress = countDirection == TimerCountDirection.CountUp ? percentage : 1.0f - percentage;
            timeRemainingNode.String = FormatTime(timeRemaining);
        }

        progressBarNode.BarColor = TimerData?.Color ?? KnownColor.CornflowerBlue.Vector();

        // Visibility based on duty/quest state
        var shouldHide = Config.HideInDuties && IDutyState.Get().IsDutyStarted;
        shouldHide |= Config.HideInQuests && IsInQuestEvent();
        IsVisible = !shouldHide;
    }

    private string FormatTime(TimeSpan timeRemaining) {
        if (TimerData?.HideSeconds ?? false) {
            return $"{timeRemaining:d\\.hh\\:mm}";
        }

        return $"{timeRemaining:d\\.hh\\:mm\\:ss}";
    }

    private static unsafe bool IsInQuestEvent() {
        var addon = RaptureAtkUnitManager.Instance()->GetAddonByName("LotO");
        return addon is not null && addon->IsVisible;
    }

    private void SavePosition() {
        var moduleName = Module.DisplayName;

        if (TimerData is not null) {
            TimerData.Position = Position;
        }
        else {
            var data = new TimerOverlayTimerData {
                Position = Position,
            };
            Config.TimerData[moduleName] = data;
        }
        Config.Save();
    }
}
