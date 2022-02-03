using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.ConfigurationSystem;
using DailyDuty.DisplaySystem.Windows;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.DisplaySystem.DisplayTabs.SettingsSubModules
{
    internal class ToDoWindowModule : DisplayModule
    {
        protected override GenericSettings GenericSettings { get; } = new();
        private readonly ToDoWindow toDoWindow = new();

        public ToDoWindowModule()
        {
            CategoryString = "Todo Pop-out Window";

            Service.WindowSystem.AddWindow(toDoWindow);
        }

        protected override void DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ShowHideWindow();

            DisableEnableClickThrough();

            TaskSelection();

            HideInDuty();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void HideInDuty()
        {
            ImGui.Checkbox("Hide when Bound By Duty", ref toDoWindow.Settings.HideInDuty);
        }

        private void TaskSelection()
        {
            ImGui.Text("Show/Hide Categories");

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            ImGui.Checkbox("Show Daily Tasks", ref toDoWindow.Settings.ShowDaily);

            ImGui.SameLine();

            ImGui.Checkbox("Show Weekly Tasks", ref toDoWindow.Settings.ShowWeekly);

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);

            ImGui.Spacing();
        }

        private void DisableEnableClickThrough()
        {
            ImGui.Text("Click-through & Lock To-Do Window");

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            if (toDoWindow.Settings.ClickThrough == true)
            {
                if (ImGui.Button("Disable", ImGuiHelpers.ScaledVector2(50, 25)))
                {
                    toDoWindow.Settings.ClickThrough = false;
                }
            }
            else
            {
                if (ImGui.Button("Enable", ImGuiHelpers.ScaledVector2(50, 25)))
                {
                    toDoWindow.Settings.ClickThrough = true;
                }
            }

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void ShowHideWindow()
        {
            ImGui.Text("To-Do Window");
            ImGui.Spacing();

            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            if (ImGui.Button($"Show##{CategoryString}", ImGuiHelpers.ScaledVector2(50, 25)))
            {
                toDoWindow.Settings.Open = true;
            }

            ImGui.SameLine();

            if (ImGui.Button($"Hide##{CategoryString}", ImGuiHelpers.ScaledVector2(50, 25)))
            {
                toDoWindow.Settings.Open = false;

            }

            ImGui.Spacing();

            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        protected override void DisplayData()
        {
        }

        protected override void EditModeOptions()
        {
        }

        protected override void NotificationOptions()
        {
        }

        public override void Dispose()
        {
            toDoWindow.Dispose();
        }
    }
}
