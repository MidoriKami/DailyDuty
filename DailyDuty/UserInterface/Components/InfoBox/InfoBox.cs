using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components.InfoBox;

public class InfoBox : DrawList<InfoBox>, IDrawable
{
    private static float CurveRadius => 13.0f * ImGuiHelpers.GlobalScale;
    private static float BorderThickness => 2.0f;
    private static int SegmentResolution => 10;
    private static float RegionWidth => 0.80f;
    private static ImDrawListPtr DrawList => ImGui.GetWindowDrawList();

    private uint ColorU32 => ImGui.GetColorU32(Color);
    private string Label { get; set; } = "Label Not Set";
    private Vector4 Color { get; } = Colors.White;
    private Vector2 Size { get; set; } = Vector2.Zero;
    private Vector2 StartPosition { get; set; }
    public float ActualWidth { get; private set; }
    public float InnerWidth { get; private set; }

    public InfoBox()
    {
        DrawListOwner = this;
    }

    public void Draw()
    {
        ImGuiHelpers.ScaledDummy(10.0f);

        var region = ImGui.GetContentRegionAvail();
        var currentPosition = ImGui.GetCursorPos();
        ActualWidth = region.X * RegionWidth;
        InnerWidth = ActualWidth - CurveRadius * 2.0f;
        
        ImGui.SetCursorPos(currentPosition with {X = region.X / 2.0f - ActualWidth / 2.0f });
        StartPosition = ImGui.GetCursorScreenPos();

        Size = new Vector2(ActualWidth, 0);

        DrawContents();

        var calculatedHeight = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + CurveRadius * 2.0f;

        Size = new Vector2(ActualWidth, calculatedHeight);

        DrawCorners();

        DrawBorders();

        ImGuiHelpers.ScaledDummy(10.0f);
    }

    private void DrawContents()
    {
        var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);

        ImGui.SetCursorScreenPos(topLeftCurveCenter);
        ImGui.PushTextWrapPos(ImGui.GetCursorPos().X + Size.X - CurveRadius * 2.0f);

        ImGui.BeginGroup();
        ImGui.PushID(Label);

        DrawListContents();

        ImGui.PopID();
        ImGui.EndGroup();

        ImGui.PopTextWrapPos();

        DrawActions.Clear();
    }

    private void DrawCorners()
    {
        var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);
        var topRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + CurveRadius);
        var bottomLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + Size.Y - CurveRadius);
        var bottomRightCurveCenter = new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + Size.Y - CurveRadius);

        DrawList.PathArcTo(topLeftCurveCenter, CurveRadius, DegreesToRadians(180), DegreesToRadians(270), SegmentResolution);
        DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(topRightCurveCenter, CurveRadius, DegreesToRadians(360), DegreesToRadians(270), SegmentResolution);
        DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(bottomLeftCurveCenter, CurveRadius, DegreesToRadians(90), DegreesToRadians(180), SegmentResolution);
        DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);

        DrawList.PathArcTo(bottomRightCurveCenter, CurveRadius, DegreesToRadians(0), DegreesToRadians(90), SegmentResolution);
        DrawList.PathStroke(ColorU32, ImDrawFlags.None, BorderThickness);
    }

    private void DrawBorders()
    {
        var color = ColorU32;

        DrawList.AddLine(new Vector2(StartPosition.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
        DrawList.AddLine(new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + CurveRadius - 0.5f), new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius + 0.5f), color, BorderThickness);
        DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius - 0.5f, StartPosition.Y + Size.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y + Size.Y - 0.5f), color, BorderThickness);

        var textSize = ImGui.CalcTextSize(Label);
        var textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
        var textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
        var textVerticalOffset = textSize.Y / 2.0f;

        DrawList.AddText(new Vector2(StartPosition.X + CurveRadius + textStartPadding, StartPosition.Y - textVerticalOffset), ColorU32, Label);
        DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius + textStartPadding + textSize.X + textEndPadding, StartPosition.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius + 0.5f, StartPosition.Y - 0.5f), color, BorderThickness);
    }

    private static float DegreesToRadians(float degrees) => MathF.PI / 180 * degrees;

    public InfoBox AddTitle(string title)
    {
        Label = title;

        return DrawListOwner;
    }

    public InfoBoxTable BeginTable(float weight = 0.50f)
    {
        return new InfoBoxTable(this, weight);
    }

    public InfoBoxList BeginList()
    {
        return new InfoBoxList(this);
    }

    public InfoBox AddList(IEnumerable<IInfoBoxListConfigurationRow> rows)
    {
        return BeginList()
            .AddRows(rows)
            .EndList();
    }
}