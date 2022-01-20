using System.Numerics;
using DailyDuty.ConfigurationSystem;
using DailyDuty.System;
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
                ImGui.Text("Daily Tasks");
                ImGui.Spacing();

                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.TreasureMap,
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings,
                    "Daily Treasure Map",
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            private void DrawWeeklyTasks()
            {
                ImGui.Text("Weekly Tasks");
                ImGui.Spacing();

                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.WondrousTails, 
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings, 
                    "Weekly Wondrous Tails", 
                    ref anyTasks);

                DrawTaskConditionally(
                    ModuleManager.ModuleType.CustomDeliveries,
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings,
                    "Weekly Custom Deliveries",
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            private void DrawTaskConditionally(ModuleManager.ModuleType type, GenericSettings settings, string text, ref bool taskSet)
            {
                if (ShowToDoTask(type, settings))
                {
                    ImGui.TextColored(new Vector4(255, 0, 0, 150), text);
                    ImGui.Spacing();
                    taskSet = true;
                }
            }

            private bool ShowToDoTask(ModuleManager.ModuleType type, GenericSettings settings)
            {
                bool moduleComplete = moduleManager[type].IsCompleted();
                bool moduleEnabled = settings.Enabled;

                return moduleEnabled && !moduleComplete;
            }

            public override void Dispose()
            {
                
            }
        }

    }
}
