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
            TabName = "ToDo List";

            FrameID = (uint) DisplayManager.Tab.ToDo;

            Modules = new()
            {
               new ToDoModule(moduleManager)
            };
        }

        public class ToDoModule : DisplayModule
        {

            private Configuration.CharacterSettings SettingsBase =>
                Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter];
            private readonly ModuleManager moduleManager;

            public ToDoModule(ModuleManager moduleManager)
            {
                this.moduleManager = moduleManager;
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

            protected override void DisplayOptions()
            {
            }

            private void DrawDailyTasks()
            {
                ImGui.Text("Daily Tasks");
                ImGui.Spacing();

                ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.TreasureMap,
                    SettingsBase.TreasureMapSettings,
                    "Treasure Map",
                    ref anyTasks);

                DrawTaskConditionally(
                    ModuleManager.ModuleType.MiniCactpot,
                    SettingsBase.MiniCactpotSettings,
                    "Mini Cactpot",
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 150), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
            }

            private void DrawWeeklyTasks()
            {
                ImGui.Text("Weekly Tasks");
                ImGui.Spacing();

                ImGui.Indent(30 * ImGuiHelpers.GlobalScale);

                bool anyTasks = false;

                DrawTaskConditionally(
                    ModuleManager.ModuleType.WondrousTails,
                    SettingsBase.WondrousTailsSettings, 
                    "Wondrous Tails", 
                    ref anyTasks);

                DrawTaskConditionally(
                    ModuleManager.ModuleType.CustomDeliveries,
                    SettingsBase.CustomDeliveriesSettings,
                    "Custom Delivery",

                    ref anyTasks);

                DrawTaskConditionally(
                    ModuleManager.ModuleType.JumboCactpot,
                    SettingsBase.JumboCactpotSettings,
                    "Jumbo Cactpot",
                    ref anyTasks);

                if (anyTasks == false)
                {
                    ImGui.TextColored(new Vector4(0, 255, 0, 150), "All Tasks Complete");
                    ImGui.Spacing();
                }

                ImGui.Indent(-30 * ImGuiHelpers.GlobalScale);
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
