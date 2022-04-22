using System;
using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Graphical
{
    internal class InfoBox : IDrawable
    {
        public Vector4 Color { get; set; } = Colors.White;
        public Action ContentsAction { get; set; } = () => { ImGui.Text("Action Not Set"); };
        public float CurveRadius { get; set; } = 15.0f;
        public Vector2 Size { get; set; } = new Vector2();
        public float BorderThickness { get; set; } = 2.0f;
        public int SegmentResolution { get; set; } = 10;
        public Vector2 Offset { get; set; }
        public string Label { get; set; } = "Label Not Set";
        public bool AutoResize = true;

        private ImDrawListPtr DrawList => ImGui.GetWindowDrawList();
        private uint ColorU32 => ImGui.GetColorU32(Color);
        private Vector2 StartPosition { get; set; }

        public bool Debug = false;

        public void Draw()
        {
            StartPosition = ImGui.GetCursorScreenPos();
            StartPosition += Offset;

            if (Debug)
            {
                DrawList.AddCircleFilled(StartPosition, 2.0f, ImGui.GetColorU32(Colors.Purple));
            }

            DrawContents();

            if (AutoResize)
            {
                Size = Size with {Y = ImGui.GetItemRectMax().Y - ImGui.GetItemRectMin().Y + CurveRadius * 2.0f};
            }

            DrawCorners();

            DrawBorders();
        }

        private void DrawContents()
        {
            var topLeftCurveCenter = new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + CurveRadius);

            ImGui.SetCursorScreenPos(topLeftCurveCenter);
            ImGui.PushTextWrapPos(Size.X);

            ImGui.BeginGroup();
            ContentsAction?.Invoke();
            ImGui.EndGroup();

            ImGui.PopTextWrapPos();
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

            if (Debug)
            {
                DrawList.AddCircleFilled(topLeftCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Red));
                DrawList.AddCircleFilled(topRightCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Green));
                DrawList.AddCircleFilled(bottomLeftCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Blue));
                DrawList.AddCircleFilled(bottomRightCurveCenter, 2.0f, ImGui.GetColorU32(Colors.Orange));
            }
        }

        private void DrawBorders()
        {
            var color = Debug ? ImGui.GetColorU32(Colors.Red) : ColorU32;

            DrawList.AddLine(new Vector2(StartPosition.X - 0.5f, StartPosition.Y + CurveRadius), new Vector2(StartPosition.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + CurveRadius), new Vector2(StartPosition.X + Size.X - 0.5f, StartPosition.Y + Size.Y - CurveRadius), color, BorderThickness);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius, StartPosition.Y + Size.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y + Size.Y - 0.5f), color, BorderThickness);

            // Top Line
            var textSize = ImGui.CalcTextSize(Label);
            var textStartPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textEndPadding = 7.0f * ImGuiHelpers.GlobalScale;
            var textVerticalOffset = textSize.Y / 2.0f;

            DrawList.AddText(new Vector2(StartPosition.X + CurveRadius + textStartPadding, StartPosition.Y - textVerticalOffset), ColorU32, Label);
            DrawList.AddLine(new Vector2(StartPosition.X + CurveRadius + textStartPadding + textSize.X + textEndPadding, StartPosition.Y - 0.5f), new Vector2(StartPosition.X + Size.X - CurveRadius, StartPosition.Y - 0.5f), color, BorderThickness);
        }

        private float DegreesToRadians(double degrees)
        {
            float radians = (float)((Math.PI / 180) * degrees);
            return (radians);
        }
    }
}
