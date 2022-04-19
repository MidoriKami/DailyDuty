using DailyDuty.Interfaces;
using ImGuiNET;

namespace DailyDuty.Windows.DailyDutyWindow.SelectionTabBar
{
    internal class FeaturesTab : ITab
    {
        public string TabName => "Features";

        public void Draw()
        {
            ImGui.Text("Oh snap, it actually worked.");
        }
    }
}
