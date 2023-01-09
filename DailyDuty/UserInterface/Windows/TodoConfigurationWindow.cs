using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Commands;
using DailyDuty.DataModels;
using DailyDuty.Localization;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.ChatCommands;
using KamiLib.Drawing;

namespace DailyDuty.UserInterface.Windows;

internal class TodoConfigurationWindow : Window
{
    private static TodoOverlaySettings Settings => Service.ConfigurationManager.CharacterConfiguration.TodoOverlay;

    public TodoConfigurationWindow() : base("DailyDuty Todo Configuration", ImGuiWindowFlags.AlwaysVerticalScrollbar)
    {
        KamiCommon.CommandManager.AddCommand(new OpenWindowCommand<TodoConfigurationWindow>("todo"));
        KamiCommon.CommandManager.AddCommand(new TodoCommands());
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 350),
            MaximumSize = new Vector2(9999,9999)
        };
    }

    public override void PreOpenCheck()
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) IsOpen = false;
        if (Service.ClientState.IsPvP) IsOpen = false;
    }

    public override void Draw()
    {

        InfoBox.Instance
            .AddTitle(Strings.Common_MainOptions)
            .AddConfigCheckbox(Strings.Common_Enabled, Settings.Enabled)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common_TaskSelection)
            .AddConfigCheckbox(Strings.Todo_ShowDailyTasks, Settings.ShowDailyTasks)
            .AddConfigCheckbox(Strings.Todo_ShowWeeklyTasks, Settings.ShowWeeklyTasks)
            .Draw();

        if (Settings.ShowDailyTasks)
        {
            var enabledDailyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Daily)
                .Where(module => module.ParentModule.GenericSettings.Enabled);

            InfoBox.Instance
                .AddTitle(Strings.Common_DailyTasks)
                .BeginTable()
                .AddConfigurationRows(enabledDailyTasks, Strings.Todo_NothingEnabled)
                .EndTable()
                .Draw();
        }

        if (Settings.ShowWeeklyTasks)
        {
            var enabledWeeklyTasks = Service.ModuleManager.GetTodoComponents(CompletionType.Weekly)
                .Where(module => module.ParentModule.GenericSettings.Enabled);

            InfoBox.Instance
                .AddTitle(Strings.Common_WeeklyTasks)
                .BeginTable()
                .AddConfigurationRows(enabledWeeklyTasks, Strings.Todo_NothingEnabled)
                .EndTable()
                .Draw();
        }

        InfoBox.Instance
            .AddTitle(Strings.Todo_TaskDisplay)
            .AddConfigCheckbox(Strings.Common_HideCompletedTasks, Settings.HideCompletedTasks)
            .AddConfigCheckbox(Strings.Todo_HideUnavailable, Settings.HideUnavailableTasks)
            .AddConfigCheckbox(Strings.Todo_ShowCompleted, Settings.ShowCategoryAsComplete)
            .Draw();

        InfoBox.Instance
            .AddTitle(Strings.Common_WindowOptions, out var innerWidth)
            .AddConfigCheckbox(Strings.Common_HideWhenComplete, Settings.HideWhenAllTasksComplete)
            .AddConfigCheckbox(Strings.Common_HideInDuty, Settings.HideWhileInDuty)
            .AddConfigCheckbox(Strings.Common_LockWindowPosition, Settings.LockWindowPosition)
            .AddConfigCheckbox(Strings.Common_AutoResize, Settings.AutoResize)
            .AddConfigCombo(Enum.GetValues<WindowAnchor>(), Settings.AnchorCorner, WindowAnchorExtensions.GetTranslatedString, Strings.Todo_AnchorCorner, innerWidth / 2.0f)
            .AddDragFloat(Strings.Common_Opacity, Settings.Opacity, 0.0f, 1.0f, innerWidth / 2.0f)
            .AddConfigColor(Strings.Common_Header, Strings.Common_Default, Settings.TaskColors.HeaderColor, Colors.White)
            .AddConfigColor(Strings.Common_Incomplete, Strings.Common_Default, Settings.TaskColors.IncompleteColor, Colors.Red)
            .AddConfigColor(Strings.Common_Complete, Strings.Common_Default, Settings.TaskColors.CompleteColor, Colors.Green)
            .AddConfigColor(Strings.Common_Unavailable, Strings.Common_Default, Settings.TaskColors.UnavailableColor, Colors.Orange)
            .Draw();
    }
    
    public override void OnClose()
    {
        Service.ConfigurationManager.Save();
    }
}