using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Configuration.Components;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components.InfoBox;

internal class InfoBox : IDrawable
{
    private static float CurveRadius => 13.0f;
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

    private readonly List<Action> drawActions = new();

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

        foreach (var action in drawActions)
        {
            action();
        }

        ImGui.PopID();
        ImGui.EndGroup();

        ImGui.PopTextWrapPos();

        drawActions.Clear();
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

    public InfoBox AddString(string message, Vector4? color = null)
    {
        drawActions.Add(Actions.GetStringAction(message, color));

        return this;
    }

    public InfoBox AddConfigCheckbox(string label, Setting<bool> setting)
    {
        drawActions.Add(Actions.GetConfigCheckboxAction(label, setting));

        return this;
    }

    public InfoBox AddConfigCombo<T>(IEnumerable<T> values, Setting<T> setting, Func<T, string> localizeFunction, string label = "", float width = 200.0f) where T : struct
    {
        drawActions.Add(Actions.GetConfigComboAction(values, setting, localizeFunction, label, width));

        return this;
    }

    public InfoBox AddConfigColor(string label, Setting<Vector4> setting)
    {
        drawActions.Add(Actions.GetConfigColor(label, setting));

        return this;
    }

    public InfoBox AddTitle(string title)
    {
        Label = title;

        return this;
    }

    public InfoBox AddDragFloat(string label, Setting<float> setting, float minValue, float maxValue, float width = 0.0f)
    {
        drawActions.Add(Actions.GetDragFloat(label, setting, minValue, maxValue, width));

        return this;
    }

    public InfoBox AddTodoComponents(IEnumerable<ITodoComponent> components, CompletionType type)
    {
        var todoComponents = components.ToList();

        if (todoComponents.Count == 0)
        {
            AddString(Strings.UserInterface.Todo.NoTasksEnabled, Colors.Orange);
        }

        foreach (var component in todoComponents)
        {
            if (component.HasLongLabel)
            {
                BeginTable()
                    .AddActions(
                        Actions.GetConfigCheckboxAction(component.ParentModule.Name.GetLocalizedString(), component.ParentModule.GenericSettings.TodoTaskEnabled),
                        Actions.GetConfigCheckboxAction(Strings.UserInterface.Todo.UseLongLabel, component.ParentModule.GenericSettings.TodoUseLongLabel))
                    .EndTable();
            }
            else
            {
                AddConfigCheckbox(component.ParentModule.Name.GetLocalizedString(), component.ParentModule.GenericSettings.TodoTaskEnabled);
            }
        }

        return this;
    }

    public InfoBox AddTimerComponents(IEnumerable<ITimerComponent> components)
    {
        var todoComponents = components.ToList();

        if (todoComponents.Count == 0)
        {
            AddString(Strings.UserInterface.Todo.NoTasksEnabled, Colors.Orange);
        }

        foreach (var component in todoComponents)
        {
            BeginTable()
                .AddActions(
                    Actions.GetConfigCheckboxAction(component.ParentModule.Name.GetLocalizedString(), component.ParentModule.GenericSettings.TimerTaskEnabled),
                    () => {
                        if (ImGui.Button(Strings.UserInterface.Timers.EditTimer))
                        {
                            Service.WindowManager.AddTimerStyleWindow(component.ParentModule, component.ParentModule.GenericSettings.TimerSettings);
                        }
                    })
            .EndTable();
        }

        return this;
    }

    public InfoBox AddAction(Action action)
    {
        drawActions.Add(action);

        return this;
    }

    public InfoBoxTable BeginTable(float weight = 0.50f)
    {
        return new InfoBoxTable(this, weight);
    }

    public InfoBox AddSliderInt(string label, Setting<int> setting, int minValue, int maxValue, float width = 200.0f)
    {
        drawActions.Add(Actions.GetSliderInt(label, setting, minValue, maxValue, width));

        return this;
    }

    public InfoBox AddConfigRadio<T>(string label, Setting<T> setting, T buttonValue, string? helpText = null ) where T : struct
    {
        drawActions.Add(Actions.GetConfigRadio(label, setting, buttonValue, helpText));

        return this;
    }
}