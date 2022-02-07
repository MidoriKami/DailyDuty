using System.Numerics;
using DailyDuty.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace DailyDuty.Components.Graphical;

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
            Service.Configuration.Save();
        }

        ImGui.SameLine(ImGui.GetWindowWidth() - 105 * ImGuiHelpers.GlobalScale);

        if (ImGui.Button($"Save & Close", ImGuiHelpers.ScaledVector2(100, 25)))
        {
            Service.Configuration.Save();
            targetWindow.IsOpen = false;
        }
    }
}