using System.Collections.Generic;
using System.Numerics;
using System.Text.Json.Serialization;
using KamiToolKit.Classes;
using KamiToolKit.Enums;

namespace DailyDuty.Features.TodoOverlay;

public class TodoPanelConfig {
    public Vector2? Position;
    public string Label = "New Panel";
    public bool IsCollapsed = false;
    public bool ShowFrame = true;
    public VerticalListAlignment Alignment = VerticalListAlignment.Left;
    public HashSet<string> Modules = [];
    public Vector4 TextColor = ColorHelper.GetColor(1);
    public Vector4 OutlineColor = ColorHelper.GetColor(53);
    public int ItemSpacing = 6;
    public float Alpha = 1.0f;
    public bool AttachToQuestList = false;

    [JsonIgnore] public bool EnableMoving;
}
