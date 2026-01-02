using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using KamiToolKit.Classes;

namespace DailyDuty.Features.TodoOverlay;

public class TodoPanelConfig {
    public Vector2? Position;
    public string Label = "New Panel";
    public bool IsCollapsed = false;
    public bool ShowFrame = true;
    public VerticalListAlignment Alignment = VerticalListAlignment.Left;
    public HashSet<string> Modules = [];

    [JsonIgnore] public bool EnableMoving;
}
