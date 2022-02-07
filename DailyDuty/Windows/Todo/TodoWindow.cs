using System;
using System.Numerics;
using DailyDuty.Data.SettingsObjects.WindowSettings;
using DailyDuty.Interfaces;
using DailyDuty.System;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Todo;

internal class TodoWindow : Window, IDisposable
{
    private readonly ITaskCategoryDisplay dailyTasks;
    private readonly ITaskCategoryDisplay weeklyTasks;

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
        dailyTasks = Service.ModuleManager.GetDailyTasks();
        weeklyTasks = Service.ModuleManager.GetWeeklyTasks();
    }

    private void Update(Framework framework)
    {
        bool dailyTasksComplete = dailyTasks.AllTasksCompleted() || !Settings.ShowDaily;
        bool weeklyTasksComplete = weeklyTasks.AllTasksCompleted() || !Settings.ShowWeekly;
        bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];

        bool hideWindow = weeklyTasksComplete && dailyTasksComplete && Settings.HideWhenTasksComplete;

        IsOpen = Settings.Open && !hideWindow && !isInQuestEvent && Service.LoggedIn;

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