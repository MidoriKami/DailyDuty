using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Enums;
using Dalamud.Interface;

namespace DailyDuty.Extensions;

public static class CompletionStatusExtensions {
    extension(CompletionStatus status) {
        public string Description => status switch {
            CompletionStatus.Unknown => "Unknown",
            CompletionStatus.Incomplete => "Incomplete",
            CompletionStatus.Unavailable => "Unavailable",
            CompletionStatus.InProgress => "In Progress",
            CompletionStatus.Complete => "Complete",
            CompletionStatus.Suppressed => "Suppressed",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
        
        public Vector4 Color => status switch {
            CompletionStatus.Unknown => KnownColor.Gray.Vector(),
            CompletionStatus.Incomplete => KnownColor.Red.Vector(),
            CompletionStatus.Unavailable => KnownColor.Orange.Vector(),
            CompletionStatus.InProgress => KnownColor.Aqua.Vector(),
            CompletionStatus.Complete => KnownColor.Green.Vector(),
            CompletionStatus.Suppressed => KnownColor.MediumPurple.Vector(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
        };
    }
}
