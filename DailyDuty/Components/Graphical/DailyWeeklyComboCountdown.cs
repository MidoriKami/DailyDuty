using DailyDuty.Interfaces;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Components.Graphical
{
    internal class DailyWeeklyComboCountdown : IDrawable
    {
        private readonly DailyResetCountdown dailyCountdown = new();
        private readonly WeeklyResetCountdown weeklyCountdown = new();

        public void Draw()
        {
            dailyCountdown.Draw();

            if (ImGui.GetWindowSize().X > 415 * ImGuiHelpers.GlobalScale)
            {
                ImGui.SameLine(ImGui.GetWindowWidth() - 205 * ImGuiHelpers.GlobalScale);
            }

            weeklyCountdown.Draw();
        }
    }
}
