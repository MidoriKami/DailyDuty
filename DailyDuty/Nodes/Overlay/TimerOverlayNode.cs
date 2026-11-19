using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features;
using DailyDuty.Interfaces;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
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

    private TimerCountDirection countDirection;
    private Vector2? pendingPosition;

    public TimerOverlayNode(IModule module, TimerOverlayConfig config) {
        Module = module;
        Config = config;

        var timerData = Config.TimerData.GetValueOrDefault(module.DisplayName);
        countDirection = timerData?.CountDirection ?? TimerCountDirection.CountDown;

        var textColor = timerData?.TextColor ?? KnownColor.White.Vector();
        var fontSize = (uint)(timerData?.FontSize ?? 16);

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
        // Always read the latest timer data from the config so UI changes are picked up
        var timerData = Config.TimerData.GetValueOrDefault(Module.DisplayName);

        // Apply pending initial position on the first frame after attachment
        if (pendingPosition.HasValue) {
            Position = pendingPosition.Value;
            pendingPosition = null;
        }

        var proposedSize = new Vector2(timerData?.Width ?? 300, timerData?.Height ?? 64);
        if (Vector2.Distance(Size, proposedSize) > 0.5f) {
            Size = proposedSize;
        }

        // Check for dirty config and apply all changes
        if (timerData is { IsDirty: true }) {
            timerData.IsDirty = false;
            countDirection = timerData.CountDirection;
            moduleNameNode.String = timerData.Label ?? Module.DisplayName;
            var textColor = timerData.TextColor;
            moduleNameNode.TextColor = textColor;
            timeRemainingNode.TextColor = textColor;
            var fontSize = (uint)timerData.FontSize;
            moduleNameNode.FontSize = fontSize;
            timeRemainingNode.FontSize = fontSize;
            progressBarNode.BarColor = timerData.Color;
        }

        var scale = timerData?.Scale ?? 1.0f;
        Scale = new Vector2(scale, scale);
        EnableMoving = timerData?.EnableMoving ?? false;

        // Update label from per-timer config
        moduleNameNode.String = timerData?.Label ?? Module.DisplayName;
        moduleNameNode.IsVisible = timerData?.ShowLabel ?? true;
        timeRemainingNode.IsVisible = timerData?.ShowCountdown ?? true;

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
        if ((timerData?.HideWhenCompleted ?? false) && timeRemaining.TotalSeconds <= 0) {
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

        progressBarNode.BarColor = timerData?.Color ?? KnownColor.CornflowerBlue.Vector();

        // Visibility based on duty/quest state
        var shouldHide = Config.HideInDuties && IDutyState.Get().IsDutyStarted;
        shouldHide |= Config.HideInQuests && ICondition.Get()[ConditionFlag.OccupiedInQuestEvent];
        IsVisible = !shouldHide;
    }

    public void SetInitialPosition(Vector2 position) {
        pendingPosition = position;
    }

    private string FormatTime(TimeSpan timeRemaining) {
        var timerData = Config.TimerData.GetValueOrDefault(Module.DisplayName);
        if (timerData?.HideSeconds == true) {
            return $"{timeRemaining:d\\.hh\\:mm}";
        }

        return $"{timeRemaining:d\\.hh\\:mm\\:ss}";
    }

    private void SavePosition() {
        var moduleName = Module.DisplayName;
        var timerData = Config.TimerData.GetValueOrDefault(moduleName);

        if (timerData is not null) {
            timerData.Position = Position;
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
