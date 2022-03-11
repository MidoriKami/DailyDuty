using System.Numerics;
using System.Reflection;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Components.Graphical
{
    internal class SaveAndCloseButtons : IDrawable
    {
        private readonly Window targetWindow;

        public SaveAndCloseButtons(Window targetWindow)
        {
            this.targetWindow = targetWindow;
        }

        public void Draw()
        {
            ImGui.SetCursorPos(new Vector2(5, ImGui.GetWindowHeight() - 30 * ImGuiHelpers.GlobalScale));

            if (ImGui.Button($"Save", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                Service.PluginInterface.UiBuilder.AddNotification("Configuration Saved", "Daily Duty", NotificationType.Success);
                Service.Configuration.Save();
            }

            ImGui.SameLine(ImGui.GetWindowWidth() - 105 * ImGuiHelpers.GlobalScale);

            if (ImGui.Button($"Save & Close", ImGuiHelpers.ScaledVector2(100, 25)))
            {
                Service.PluginInterface.UiBuilder.AddNotification("Configuration Saved", "Daily Duty", NotificationType.Success);
                Service.Configuration.Save();
                targetWindow.IsOpen = false;
            }

            if (Service.Configuration.System.ShowVersionNumber)
            {
                DrawVersionNumber();
            }
        }

        private void DrawVersionNumber()
        {
            var x = ImGui.GetWindowWidth() / 2 - 43 * ImGuiHelpers.GlobalScale;
            var y = ImGui.GetWindowHeight() - 25 * ImGuiHelpers.GlobalScale;
            
            ImGui.SetCursorPos(new Vector2(x, y));

            var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

            ImGui.TextColored(Colors.Grey, assemblyInformation[1].Replace('=', ' '));
        }
    }
}