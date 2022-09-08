using System.Numerics;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.UserInterface.Windows;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.UserInterface.Components;

internal class NavigationButtons : IDrawable
{
    public void Draw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(2.0f, 0.0f) * ImGuiHelpers.GlobalScale);

        var contentRegion = ImGui.GetContentRegionAvail();
        var buttonWidth = contentRegion.X / 3.0f - 2.0f * ImGuiHelpers.GlobalScale;

        if (ImGui.Button(Strings.UserInterface.Todo.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<TodoConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.SameLine();

        if (ImGui.Button(Strings.UserInterface.Timers.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<TimersConfigurationWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.SameLine();

        if (ImGui.Button(Strings.Status.Label, new Vector2(buttonWidth, 23.0f * ImGuiHelpers.GlobalScale)))
        {
            var window = Service.WindowManager.GetWindowOfType<StatusWindow>()!;
            window.IsOpen = !window.IsOpen;
        }

        ImGui.PopStyleVar();
    }
}