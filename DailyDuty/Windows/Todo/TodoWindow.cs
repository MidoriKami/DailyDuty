using System;
using System.Numerics;
using DailyDuty.Components.Graphical;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.Todo
{
    internal class TodoWindow : Window, IDisposable, IWindow
    {
        private readonly ITaskCategoryDisplay dailyTasks;
        private readonly ITaskCategoryDisplay weeklyTasks;
        private int frameCounter = 0;
        private Vector2 lastWindowSize = Vector2.Zero;

        public new WindowName WindowName => WindowName.Todo;

        private TodoWindowSettings Settings => Service.Configuration.Windows.Todo;

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
            var clr = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(clr.X, clr.Y, clr.Z, Settings.Opacity));

            clr = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(clr.X, clr.Y, clr.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            if(Settings.Anchor != WindowAnchor.TopLeft)
            {
                var size = ImGui.GetWindowSize();
                if(lastWindowSize != Vector2.Zero) {
                    var offset = lastWindowSize - size;

                    if(!Settings.Anchor.HasFlag(WindowAnchor.Right))
                        offset.X = 0;

                    if(!Settings.Anchor.HasFlag(WindowAnchor.Bottom))
                        offset.Y = 0;

                    ImGui.SetWindowPos(ImGui.GetWindowPos() + offset);
                }
                lastWindowSize = size;
            }

            bool dailyTasksComplete = dailyTasks.AllTasksCompleted() || !Settings.ShowDaily;
            bool hideDailyTasks = Settings.HideWhenTasksComplete && dailyTasksComplete;

            bool weeklyTasksComplete = weeklyTasks.AllTasksCompleted() || !Settings.ShowWeekly;
            bool hideWeeklyTasks = Settings.HideWhenTasksComplete && weeklyTasksComplete;

            if(Settings.ShowDaily && !hideDailyTasks)
                dailyTasks.Draw();

            ImGui.Spacing();

            if(Settings.ShowWeekly && !hideWeeklyTasks)
                weeklyTasks.Draw();
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor(2);
        }

        public void Dispose()
        {
            Service.Framework.Update -= Update;

            Service.WindowSystem.RemoveWindow(this);
        }
    }
}