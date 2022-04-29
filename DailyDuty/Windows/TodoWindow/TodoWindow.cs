using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.TodoWindow
{
    internal class TodoWindow : Window, IDisposable
    {
        private readonly TaskCategoryDisplay dailyTasks = new()
        {
            Tasks = Service.ModuleManager.GetCompletables(CompletionType.Daily).OrderBy(task => task.DisplayName).ToList(),
            HeaderText = Strings.Common.DailyTasksLabel,
            Colors = Service.SystemConfiguration.Windows.Todo.Colors
        };

        private readonly TaskCategoryDisplay weeklyTasks = new()
        {
            Tasks = Service.ModuleManager.GetCompletables(CompletionType.Weekly).OrderBy(task => task.DisplayName).ToList(),
            HeaderText = Strings.Common.WeeklyTasksLabel,
            Colors = Service.SystemConfiguration.Windows.Todo.Colors
        };

        private Vector2 lastWindowSize = Vector2.Zero;

        private TodoWindowSettings Settings => Service.SystemConfiguration.Windows.Todo;

        public TodoWindow() : base("DailyDuty Todo List")
        {
            Service.WindowSystem.AddWindow(this);
        }

        public override void PreOpenCheck()
        {
            if (!Service.LoggedIn || Service.ClientState.IsPvP)
            {
                IsOpen = false;
                return;
            }

            bool dailyTasksComplete = dailyTasks.AllTasksCompleted() || !Settings.ShowDaily;
            bool weeklyTasksComplete = weeklyTasks.AllTasksCompleted() || !Settings.ShowWeekly;
            bool tasksComplete = dailyTasksComplete && weeklyTasksComplete;

            bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];

            bool hideWindow = tasksComplete && Settings.HideWhenTasksComplete;

            IsOpen = Settings.Enabled && !hideWindow && !isInQuestEvent && Service.LoggedIn;

            if (Settings.HideInDuty && Utilities.Condition.IsBoundByDuty())
            {
                IsOpen = false;
            }
        }
        
        public override void PreDraw()
        {
            var bgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg];
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(bgColor.X, bgColor.Y, bgColor.Z, Settings.Opacity));

            var borderColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Border];
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(borderColor.X, borderColor.Y, borderColor.Z, Settings.Opacity));
        }

        public override void Draw()
        {
            if (IsOpen == false) return;

            switch (Settings.Style)
            {
                case TodoWindowStyle.AutoResize:
                    Flags = DrawFlags.AutoResize;
                    Flags |= Settings.ClickThrough ? ImGuiWindowFlags.NoInputs : ImGuiWindowFlags.None;
                    break;

                case TodoWindowStyle.ManualSize:
                    Flags = DrawFlags.ManualSize;
                    Flags |= Settings.ClickThrough ? DrawFlags.LockPosition : ImGuiWindowFlags.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(Settings.Anchor != WindowAnchor.TopLeft && Settings.Style != TodoWindowStyle.ManualSize)
            {
                var size = ImGui.GetWindowSize();

                if(lastWindowSize != Vector2.Zero) 
                {
                    var offset = lastWindowSize - size;

                    if(!Settings.Anchor.HasFlag(WindowAnchor.TopRight))
                        offset.X = 0;

                    if(!Settings.Anchor.HasFlag(WindowAnchor.BottomLeft))
                        offset.Y = 0;

                    if (offset != Vector2.Zero)
                    {
                        ImGui.SetWindowPos(ImGui.GetWindowPos() + offset);
                    }
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
            Service.WindowSystem.RemoveWindow(this);
        }
    }
}