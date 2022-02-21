using System;
using System.Numerics;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.WindowSettings;
using DailyDuty.Interfaces;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Todo;

internal class TodoWindow : Window, IDisposable, IWindow
{
    private readonly ITaskCategoryDisplay dailyTasks;
    private readonly ITaskCategoryDisplay weeklyTasks;
    private int frameCounter = 0;

    public new WindowName WindowName => WindowName.Todo;

    private TodoWindowSettings Settings => Service.Configuration.TodoWindowSettings;

    private const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                  ImGuiWindowFlags.NoTitleBar |
                                                  ImGuiWindowFlags.NoScrollbar |
                                                  ImGuiWindowFlags.NoCollapse |
                                                  ImGuiWindowFlags.AlwaysAutoResize;

    private const ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                       ImGuiWindowFlags.NoDecoration |
                                                       ImGuiWindowFlags.NoInputs |
                                                       ImGuiWindowFlags.AlwaysAutoResize;

    public TodoWindow() : base("DailyDuty Todo List")
    {
        Service.WindowSystem.AddWindow(this);

        Service.Framework.Update += Update;

        var dailyCompletables = Service.ModuleManager.GetCompletables(CompletionType.Daily);
        var weeklyCompletables = Service.ModuleManager.GetCompletables(CompletionType.Weekly);

        dailyTasks = new FormattedDailyTasks(dailyCompletables);
        weeklyTasks = new FormattedWeeklyTasks(weeklyCompletables);
    }

    private void Update(Framework framework)
    {
        if (Service.LoggedIn == false)
        {
            IsOpen = false;
            return;
        }

        if(frameCounter++ % 10 != 0) return;

        bool dailyTasksComplete = dailyTasks.AllTasksCompleted() || !Settings.ShowDaily;
        bool weeklyTasksComplete = weeklyTasks.AllTasksCompleted() || !Settings.ShowWeekly;
        bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];

        bool hideWindow = weeklyTasksComplete && dailyTasksComplete && Settings.HideWhenTasksComplete;

        IsOpen = Settings.Open && !hideWindow && !isInQuestEvent;

        if (Settings.HideInDuty == true)
        {
            if (Utilities.Condition.IsBoundByDuty() == true)
            {
                IsOpen = false;
            }
        }

        Flags = Settings.ClickThrough ? ClickThroughFlags : DefaultFlags;
    }

    public override void PreDraw()
    {
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, Settings.Opacity));
    }

    public override void Draw()
    {
        bool dailyTasksComplete = dailyTasks.AllTasksCompleted() || !Settings.ShowDaily;
        bool hideDailyTasks = Settings.HideWhenTasksComplete && dailyTasksComplete;

        bool weeklyTasksComplete = weeklyTasks.AllTasksCompleted() || !Settings.ShowWeekly;
        bool hideWeeklyTasks = Settings.HideWhenTasksComplete && weeklyTasksComplete;

        if(Settings.ShowDaily && !hideDailyTasks)
            dailyTasks.Draw();

        if (Settings.ShowWeekly && !hideWeeklyTasks)
            weeklyTasks.Draw();
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
    }

    public void Dispose()
    {
        Service.Framework.Update -= Update;

        Service.WindowSystem.RemoveWindow(this);
    }
}