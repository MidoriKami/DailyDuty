using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using DailyDuty.Windows.DailyDutyWindow.SelectionTabBar;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Windows.DailyDutyWindow
{
    internal class DailyDutyWindow : Window, IDisposable, ICommand
    {
        private readonly ITabBar tabBar = new ModuleSelectionTabBar();

        public DailyDutyWindow() : base("DailyDuty Window")
        {
            Service.WindowSystem.AddWindow(this);

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new(700, 400),
                MaximumSize = new(9999,9999)
            };

            Flags |= ImGuiWindowFlags.NoScrollbar;
            Flags |= ImGuiWindowFlags.NoScrollWithMouse;
        }

        public void Dispose()
        {
            Service.WindowSystem.RemoveWindow(this);
        }

        public override void Draw()
        {
            if (!IsOpen) return;

            var availableArea = ImGui.GetContentRegionAvail();

            var ratio = 0.50f;
            var padding = 4.0f;

            var moduleSelectionWidth = availableArea.X * (ratio) - padding;

            var moduleSettingsWidth = availableArea.X * (1.0f - ratio) - padding;

            if (ImGui.BeginChild("ModuleSelection", ImGuiHelpers.ScaledVector2(moduleSelectionWidth, availableArea.Y), true))
            {
                tabBar.Draw();

                ImGui.EndChild();
            }

            ImGui.SameLine();

            if (ImGui.BeginChild("ModuleSettings", ImGuiHelpers.ScaledVector2(moduleSettingsWidth, availableArea.Y), true))
            {


                ImGui.EndChild();
            }
        }

        void ICommand.Execute(string? primaryCommand, string? secondaryCommand)
        {
            if (primaryCommand == null)
            {
                Toggle();
            }
        }
    }
}
