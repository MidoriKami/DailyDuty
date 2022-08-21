using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Configuration;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.OverlaySettings;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class TodoOverlayWindow : Window, IDisposable
{
    private static TodoOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TodoOverlay;

    private Vector2 lastWindowSize = Vector2.Zero;
    private List<ITodoComponent> trackedTasks = new();

    public TodoOverlayWindow() : base($"###DailyDutyTodoOverlayWindow+{Service.ConfigurationManager.CharacterConfiguration.CharacterData.Name}")
    {
        Service.ConfigurationManager.OnCharacterDataAvailable += UpdateWindowTitle;

    }

    public void Dispose()
    {
        Service.ConfigurationManager.OnCharacterDataAvailable -= UpdateWindowTitle;
    }

    private void UpdateWindowTitle(object? sender, CharacterConfiguration e)
    {
        WindowName = $"###DailyDutyTodoOverlayWindow+{e.CharacterData.Name}";
    }

    public override void PreOpenCheck()
    {
        if (Settings.Enabled.Value) IsOpen = true;
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
        if (Condition.InCutsceneOrQuestEvent()) IsOpen = false;
        if (Condition.IsBoundByDuty() && Settings.HideWhileInDuty.Value) IsOpen = false;

        trackedTasks = GetTrackedTasks().ToList();
        if (Settings.HideWhenAllTasksComplete.Value && !trackedTasks.Any()) IsOpen = false;
    }

    public override void PreDraw()
    {
        var bgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
        ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor with {W = Settings.Opacity.Value});

        var borderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor with {W = Settings.Opacity.Value});
    }

    public override void Draw()
    {
        ResizeWindow();

        DrawDailyTasks();

        DrawWeeklyTasks();
    }

    private void ResizeWindow()
    {
        if (Settings.AutoResize.Value)
        {
            Flags = DrawFlags.AutoResize;
            Flags |= Settings.LockWindowPosition.Value ? ImGuiWindowFlags.NoInputs : ImGuiWindowFlags.None;
        }
        else
        {
            Flags = DrawFlags.ManualSize;
            Flags |= Settings.LockWindowPosition.Value ? DrawFlags.LockPosition : ImGuiWindowFlags.None;
        }

        if(Settings.AnchorCorner.Value != WindowAnchor.TopLeft && Settings.AutoResize.Value)
        {
            var size = ImGui.GetWindowSize();

            if(lastWindowSize != Vector2.Zero) 
            {
                var offset = lastWindowSize - size;

                if(!Settings.AnchorCorner.Value.HasFlag(WindowAnchor.TopRight))
                    offset.X = 0;

                if(!Settings.AnchorCorner.Value.HasFlag(WindowAnchor.BottomLeft))
                    offset.Y = 0;

                if (offset != Vector2.Zero)
                {
                    ImGui.SetWindowPos(ImGui.GetWindowPos() + offset);
                }
            }

            lastWindowSize = size;
        }
    }

    private void DrawDailyTasks()
    {
        var dailyTasks = trackedTasks.Where(module => module.CompletionType == CompletionType.Daily).ToList();

        if (!dailyTasks.Any()) return;

        ImGui.TextColored(Settings.TaskColors.HeaderColor.Value, Strings.UserInterface.Todo.DailyTasks);

        ImGui.Indent(30.0f * ImGuiHelpers.GlobalScale);

        DrawTasks(dailyTasks);

        ImGui.Indent(-30.0f * ImGuiHelpers.GlobalScale);
    }

    private void DrawWeeklyTasks()
    {
        var weeklyTasks = trackedTasks.Where(module => module.CompletionType == CompletionType.Weekly).ToList();

        if (!weeklyTasks.Any()) return;

        ImGui.TextColored(Settings.TaskColors.HeaderColor.Value, Strings.UserInterface.Todo.WeeklyTasks);

        ImGui.Indent(30.0f * ImGuiHelpers.GlobalScale);

        DrawTasks(weeklyTasks);

        ImGui.Indent(-30.0f * ImGuiHelpers.GlobalScale);
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
    }

    private IEnumerable<ITodoComponent> GetTrackedTasks()
    {
        var tasks = new List<ITodoComponent>();

        if(Settings.ShowDailyTasks.Value)
            tasks.AddRange(Service.ModuleManager.GetTodoComponents(CompletionType.Daily));

        if(Settings.ShowWeeklyTasks.Value)
            tasks.AddRange(Service.ModuleManager.GetTodoComponents(CompletionType.Weekly));

        tasks.RemoveAll(module => !module.ParentModule.GenericSettings.Enabled.Value);

        tasks.RemoveAll(module => !module.Enabled.Value);

        if (Settings.HideCompletedTasks.Value)
            tasks.RemoveAll(module =>
                module.ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Complete);

        if (Settings.HideUnavailableTasks.Value)
            tasks.RemoveAll(module =>
                module.ParentModule.LogicComponent.GetModuleStatus() == ModuleStatus.Unavailable);

        return tasks;
    }

    private static void DrawTasks(List<ITodoComponent> dailyTasks)
    {
        foreach (var task in dailyTasks)
        {
            var taskLabel = task.GetLongTaskLabel();

            switch (task.ParentModule.LogicComponent.GetModuleStatus())
            {
                case ModuleStatus.Incomplete:
                    ImGui.TextColored(Settings.TaskColors.IncompleteColor.Value, taskLabel);
                    break;

                case ModuleStatus.Unavailable:
                    ImGui.TextColored(Settings.TaskColors.UnavailableColor.Value, taskLabel);
                    break;

                case ModuleStatus.Complete:
                    ImGui.TextColored(Settings.TaskColors.CompleteColor.Value, taskLabel);
                    break;
            }
        }
    }
}