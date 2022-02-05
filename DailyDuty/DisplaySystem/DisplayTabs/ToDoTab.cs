using System.Linq;
using System.Numerics;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.Windows;
using DailyDuty.System;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class ToDoTab : TabCategory
    {
        public ToDoTab()
        {
            CategoryName = "Outstanding Tasks";
            TabName = "ToDo List";

            FrameID = (uint) DisplayManager.Tab.ToDo;

            Modules = new()
            {
               new ToDoModule()
            };
        }

        public class ToDoModule : DisplayModule
        {

            public bool WeeklyTasksComplete = false;
            public bool DailyTasksComplete = false;

            private Configuration.CharacterSettings SettingsBase =>
                Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter];
            private readonly ModuleManager moduleManager;

            public ToDoModule()
            {
                this.moduleManager = Service.ModuleManager;
            }

            protected override void EditModeOptions()
            {
            }

            protected override void NotificationOptions()
            {
            }

            public override void Draw()
            {
                ImGui.Spacing();

                DrawContents();

                ImGui.Spacing();
            }

            protected override GenericSettings GenericSettings { get; } = new();

            protected override void DrawContents()
            {
                DrawDailyTasks();

                DrawWeeklyTasks();
            }

            protected override void DisplayData()
            {
            }

            public void DrawDailyTasks()
            {
                ImGui.Text("Daily Tasks");
                ImGui.Spacing();

                ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

                foreach (var module in Service.ModuleManager.GetModulesByType(ModuleManager.ModuleType.Daily))
                {
                    DrawTaskStatus(module);
                }

                bool allTasksComplete = Service.ModuleManager.GetModulesByType(ModuleManager.ModuleType.Daily)
                    .Where(task => task.GenericSettings.Enabled)
                    .All(enabledTasks => enabledTasks.IsCompleted());

                if (allTasksComplete == true)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 150), "All Tasks Complete");
                    ImGui.Spacing();
                }

                DailyTasksComplete = allTasksComplete;

                ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
            }

            public void DrawWeeklyTasks()
            {
                ImGui.Text("Weekly Tasks");
                ImGui.Spacing();

                ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

                foreach (var module in Service.ModuleManager.GetModulesByType(ModuleManager.ModuleType.Weekly))
                {
                    DrawTaskStatus(module);
                }

                bool allTasksComplete = Service.ModuleManager.GetModulesByType(ModuleManager.ModuleType.Weekly)
                    .Where(task => task.GenericSettings.Enabled)
                    .All(enabledTasks => enabledTasks.IsCompleted());

                if (allTasksComplete == true)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 150), "All Tasks Complete");
                    ImGui.Spacing();
                }

                WeeklyTasksComplete = allTasksComplete;

                ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
            }

            private void DrawTaskStatus(Module taskModule)
            {
                if (taskModule.IsCompleted() == false && taskModule.GenericSettings.Enabled)
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), taskModule.ModuleName);
                    ImGui.Spacing();
                }
            }


            //private void DrawTaskConditionally(ModuleManager.ModuleType type, GenericSettings settings, string text, ref bool taskSet)
            //{
            //    if (ShowToDoTask(type, settings))
            //    {
            //        ImGui.TextColored(new Vector4(255, 0, 0, 150), text);
            //        ImGui.Spacing();
            //        taskSet = true;
            //    }
            //}

            //private bool ShowToDoTask(ModuleManager.ModuleType type, GenericSettings settings)
            //{
            //    bool moduleComplete = moduleManager[type].IsCompleted();
            //    bool moduleEnabled = settings.Enabled;

            //    return moduleEnabled && !moduleComplete;
            //}

            public override void Dispose()
            {
                
            }
        }

    }
}
