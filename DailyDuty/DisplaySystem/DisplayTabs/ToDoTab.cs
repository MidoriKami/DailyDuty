using System.Linq.Expressions;
using System.Numerics;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System;
using DailyDuty.System.Modules;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs
{
    internal class ToDoTab : TabCategory
    {

        public ToDoTab(ModuleManager moduleManager)
        {
            CategoryName = "Outstanding Tasks";
            TabName = "Todo List";

            FrameID = (uint) DisplayManager.Tab.ToDo;

            Modules = new()
            {
                {"ToDo", new ToDoModule(moduleManager) }
            };
        }

        public class ToDoModule : DisplayModule
        {
            private readonly ModuleManager moduleManager;

            public ToDoModule(ModuleManager moduleManager)
            {
                this.moduleManager = moduleManager;
            }

            public override void Draw()
            {
                ImGui.Spacing();

                DrawContents();

                ImGui.Spacing();
            }

            protected override void DrawContents()
            {
                DrawDailyTasks();

                DrawWeeklyTasks();
            }

            private void DrawDailyTasks()
            {
                DrawSeparatedText("Daily Tasks");
                
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                if (ShowToDoTask(ModuleManager.ModuleType.TreasureMap, Service.Configuration.TreasureMapSettings))
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), "Daily Treasure Map");
                    ImGui.Spacing();
                    anyTasks = true;
                }

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            private void DrawWeeklyTasks()
            {
                DrawSeparatedText("Weekly Tasks");
                
                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                if ( ShowToDoTask(ModuleManager.ModuleType.WondrousTails, Service.Configuration.WondrousTailsSettings) )
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), "Weekly Wondrous Tails");
                    ImGui.Spacing();
                    anyTasks = true;
                }

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            private static void DrawSeparatedText(string text)
            {
                ImGui.Text(text);
                ImGui.Separator();
                ImGui.Spacing();
            }

            private bool ShowToDoTask(ModuleManager.ModuleType type, GenericSettings settings)
            {
                bool moduleComplete = moduleManager[type].IsCompleted();
                bool moduleEnabled = settings.Enabled;

                return moduleEnabled && moduleComplete;
            }

            public override void Dispose()
            {
                
            }
        }

    }
}
