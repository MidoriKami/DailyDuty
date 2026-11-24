using System.Numerics;
using System.Text.Json.Serialization;
using DailyDuty.Enums;

namespace DailyDuty.UI;

public class TimerOverlayTimerData {
    public Vector2 Position { get; set; }
    public Vector4 Color { get; set; } = new(0.388f, 0.573f, 0.929f, 1.0f);
    public Vector4 TextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public string? Label { get; set; }
    public TimerCountDirection CountDirection { get; set; }
    public int FontSize { get; set; } = 16;
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool ShowLabel { get; set; } = true;
    public bool ShowCountdown { get; set; } = true;
    public bool HideSeconds { get; set; }
    public bool HideWhenCompleted { get; set; }
    public float Scale { get; set; } = 1.0f;

    [JsonIgnore]
    public bool EnableMoving { get; set; }

    [JsonIgnore]
    public bool IsDirty { get; set; }
}
