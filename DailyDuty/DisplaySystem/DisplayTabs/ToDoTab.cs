using System.Numerics;
using CheapLoc;
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
            CategoryName = Loc.Localize("Outstanding Tasks", "Outstanding Tasks");
            TabName = Loc.Localize("Todo List", "Todo List");

            FrameID = (uint) DisplayManager.Tab.ToDo;

            Modules = new()
            {
               new ToDoModule(moduleManager)
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
                ImGui.Text(Loc.Localize("Daily Tasks", "Daily Tasks"));
                ImGui.Spacing();

                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.TreasureMap,
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].TreasureMapSettings,
                    Loc.Localize("Daily Treasure Map", "Daily Treasure Map"),
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), Loc.Localize("All Tasks Complete", "All Tasks Complete"));
                    ImGui.Spacing();
                }

                ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
            }

            private void DrawWeeklyTasks()
            {
                ImGui.Text(Loc.Localize("Weekly Tasks", "Weekly Tasks"));
                ImGui.Spacing();

                ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.WondrousTails, 
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].WondrousTailsSettings, 
                    Loc.Localize("Weekly Wondrous Tails", "Weekly Wondrous Tails"), 
                    ref anyTasks);

                DrawTaskConditionally(
                    ModuleManager.ModuleType.CustomDeliveries,
                    Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings,
                    Loc.Localize("Weekly Custom Deliveries", "Weekly Custom Deliveries"),
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 255), Loc.Localize("All Tasks Complete", "All Tasks Complete"));
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
