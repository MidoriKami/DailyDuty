using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.OverlaySettings;
using DailyDuty.Modules.Enums;
using DailyDuty.System.Localization;
using DailyDuty.UserInterface.Components.InfoBox;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.UserInterface.Windows;

internal class TodoConfigurationWindow : Window, IDisposable
{
    public static TodoOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TodoOverlay;

    private readonly InfoBox mainOptionsInfoBox = new();
    private readonly InfoBox taskSelectionsInfoBox = new();
    private readonly InfoBox taskHidingInfoBox = new();
    private readonly InfoBox windowHidingOptionsInfoBox = new();
    private readonly InfoBox windowPositionOptionsInfoBox = new();
    private readonly InfoBox categoryColorsInfoBox = new();
    private readonly InfoBox dailyTasksInfoBox = new();
    private readonly InfoBox weeklyTasksInfoBox = new();

    public TodoConfigurationWindow() : base("DailyDuty Todo Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350 * (4.0f / 3.0f), 350),
            MaximumSize = new Vector2(9999,9999)
        };
    }

    public void Dispose()
    {

    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
    }

    public override void Draw()
    {
        mainOptionsInfoBox
            .AddTitle(Strings.UserInterface.Todo.MainOptions)
            .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
            .Draw();

        taskSelectionsInfoBox
            .AddTitle(Strings.UserInterface.Todo.TaskSelection)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowDailyTasks, Settings.ShowDailyTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowWeeklyTasks, Settings.ShowWeeklyTasks)
            .Draw();

        if (Settings.ShowDailyTasks.Value)
        {
            var enabledDailyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Daily)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            dailyTasksInfoBox
                .AddTitle(Strings.UserInterface.Todo.DailyTasks)
                .AddTodoComponents(enabledDailyTasks, CompletionType.Daily)
                .Draw();
        }

        if (Settings.ShowWeeklyTasks.Value)
        {
            var enabledWeeklyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Weekly)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            weeklyTasksInfoBox
                .AddTitle(Strings.UserInterface.Todo.WeeklyTasks)
                .AddTodoComponents(enabledWeeklyTasks, CompletionType.Weekly)
                .Draw();
        }

        taskHidingInfoBox
            .AddTitle(Strings.UserInterface.Todo.TaskDisplay)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideCompletedTasks, Settings.HideCompletedTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideUnavailable, Settings.HideUnavailableTasks)
            .Draw();

        windowHidingOptionsInfoBox
            .AddTitle(Strings.UserInterface.Todo.WindowOptions)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowCompleted, Settings.HideWhenAllTasksComplete)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowInDuty, Settings.HideWhileInDuty)
            .Draw();

        windowPositionOptionsInfoBox
            .AddTitle(Strings.UserInterface.Todo.PositionOptions)
            .AddConfigCheckbox(Strings.UserInterface.Todo.LockWindow, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.UserInterface.Todo.AutoResize, Settings.AutoResize)
            .AddConfigCombo(Enum.GetValues<WindowAnchor>(), Settings.AnchorCorner, WindowAnchorExtensions.GetLocalizedString, Strings.UserInterface.Todo.AnchorCorner)
            .AddDragFloat(Strings.UserInterface.Todo.Opacity, Settings.Opacity, 0.0f, 1.0f, 200.0f)
            .Draw();

        categoryColorsInfoBox
            .AddTitle(Strings.UserInterface.Todo.ColorOptions)
            .AddConfigColor(Strings.Common.Header, Settings.TaskColors.HeaderColor)
            .AddConfigColor(Strings.Common.Incomplete, Settings.TaskColors.IncompleteColor)
            .AddConfigColor(Strings.Common.Unavailable, Settings.TaskColors.UnavailableColor)
            .AddConfigColor(Strings.Common.Complete, Settings.TaskColors.CompleteColor)
            .Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleVar();
    }

    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}