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

    private readonly InfoBox mainOptions = new();
    private readonly InfoBox taskSelections = new();
    private readonly InfoBox taskHiding = new();
    private readonly InfoBox windowHidingOptions = new();
    private readonly InfoBox dailyTasks = new();
    private readonly InfoBox weeklyTasks = new();

    public TodoConfigurationWindow() : base("DailyDuty Todo Configuration", ImGuiWindowFlags.AlwaysVerticalScrollbar)
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
        ImGui.PushID("TodoConfiguration");

    }

    public override void Draw()
    {

        mainOptions
            .AddTitle(Strings.UserInterface.Todo.MainOptions)
            .AddConfigCheckbox(Strings.Common.Enabled, Settings.Enabled)
            .Draw();

        taskSelections
            .AddTitle(Strings.UserInterface.Todo.TaskSelection)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowDailyTasks, Settings.ShowDailyTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.ShowWeeklyTasks, Settings.ShowWeeklyTasks)
            .Draw();

        if (Settings.ShowDailyTasks.Value)
        {
            var enabledDailyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Daily)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            dailyTasks
                .AddTitle(Strings.UserInterface.Todo.DailyTasks)
                .AddTodoComponents(enabledDailyTasks, CompletionType.Daily)
                .Draw();
        }

        if (Settings.ShowWeeklyTasks.Value)
        {
            var enabledWeeklyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Weekly)
                .Where(module => module.ParentModule.GenericSettings.Enabled.Value);

            weeklyTasks
                .AddTitle(Strings.UserInterface.Todo.WeeklyTasks)
                .AddTodoComponents(enabledWeeklyTasks, CompletionType.Weekly)
                .Draw();
        }

        taskHiding
            .AddTitle(Strings.UserInterface.Todo.TaskDisplay)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideCompletedTasks, Settings.HideCompletedTasks)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideUnavailable, Settings.HideUnavailableTasks)
            .Draw();

        windowHidingOptions
            .AddTitle(Strings.UserInterface.Todo.WindowOptions)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowCompleted, Settings.HideWhenAllTasksComplete)
            .AddConfigCheckbox(Strings.UserInterface.Todo.HideWindowInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.UserInterface.Todo.LockWindow, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.UserInterface.Todo.AutoResize, Settings.AutoResize)
            .AddConfigCombo(Enum.GetValues<WindowAnchor>(), Settings.AnchorCorner, WindowAnchorExtensions.GetLocalizedString, Strings.UserInterface.Todo.AnchorCorner)
            .AddDragFloat(Strings.UserInterface.Todo.Opacity, Settings.Opacity, 0.0f, 1.0f, 200.0f)
            .AddConfigColor(Strings.Common.Header, Settings.TaskColors.HeaderColor)
            .AddConfigColor(Strings.Common.Incomplete, Settings.TaskColors.IncompleteColor)
            .AddConfigColor(Strings.Common.Unavailable, Settings.TaskColors.UnavailableColor)
            .AddConfigColor(Strings.Common.Complete, Settings.TaskColors.CompleteColor)
            .Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopID();
    }

    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}