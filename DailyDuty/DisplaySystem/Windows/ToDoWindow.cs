using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.DisplayTabs;
using DailyDuty.System;
using DailyDuty.System.Utilities;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.Windows
{
    public class ToDoWindow : Window, IDisposable
    {
        public ToDoWindowSettings Settings => Service.Configuration.ToDoWindowSettings;

        private ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                ImGuiWindowFlags.NoTitleBar |
                                                ImGuiWindowFlags.NoScrollbar |
                                                ImGuiWindowFlags.NoCollapse;

        private ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                     ImGuiWindowFlags.NoDecoration |
                                                     ImGuiWindowFlags.NoInputs;

        private readonly ToDoTab.ToDoModule toDoTab = new();

        public ToDoWindow() : base("DailyDuty ToDo Window")
        {
            Service.Framework.Update += FrameWorkUpdate;
        }

        private void FrameWorkUpdate(Framework framework)
        {
            
            bool dailyTasksComplete = Service.ModuleManager.TasksCompleteByType(ModuleManager.ModuleType.Daily) || !Settings.ShowDaily;
            bool weeklyTasksComplete = Service.ModuleManager.TasksCompleteByType(ModuleManager.ModuleType.Weekly) || !Settings.ShowWeekly;
            bool isInQuestEvent = Service.Condition[ConditionFlag.OccupiedInQuestEvent];

            bool hideWindow = weeklyTasksComplete && dailyTasksComplete && Settings.HideWhenTasksComplete;

            IsOpen = Settings.Open && !hideWindow && !isInQuestEvent && Service.LoggedIn;

            if (Settings.HideInDuty == true)
            {
                if (ConditionManager.IsBoundByDuty() == true)
                {
                    IsOpen = false;
                }
            }

            Flags = Settings.ClickThrough ? ClickThroughFlags : DefaultFlags;
        }

        public override void PreDraw()
        {
            base.PreDraw();

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, Settings.Opacity));
        }

        public override void Draw()
        {
            bool dailyTasksComplete = Service.ModuleManager.TasksCompleteByType(ModuleManager.ModuleType.Daily);
            bool hideDailyTasks = Settings.HideWhenTasksComplete && dailyTasksComplete;

            bool weeklyTasksComplete = Service.ModuleManager.TasksCompleteByType(ModuleManager.ModuleType.Weekly);
            bool hideWeeklyTasks = Settings.HideWhenTasksComplete && weeklyTasksComplete;

            if(Settings.ShowDaily && !hideDailyTasks)
                toDoTab.DrawDailyTasks();

            if (Settings.ShowWeekly && !hideWeeklyTasks)
                toDoTab.DrawWeeklyTasks();
        }

        public override void PostDraw()
        {
            base.PostDraw();

            ImGui.PopStyleColor();
        }

        public void Dispose()
        {
            Service.Framework.Update -= FrameWorkUpdate;

            Settings.Open = IsOpen;
            Service.Configuration.Save();
            toDoTab.Dispose();
        }
    }
}
