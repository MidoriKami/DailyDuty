using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ITab
    {
        ITabItem? SelectedTabItem { get; set; }
        List<ITabItem> TabItems { get; set; }
        string TabName { get; }
        string Description { get; }

        public void DrawTabContents()
        {
            var frameBgColor = ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBg];
            ImGui.PushStyleColor(ImGuiCol.FrameBg, frameBgColor with {W = 0.05f});

            ImGui.BeginListBox("", new Vector2(-1, -1));

            ImGui.PopStyleColor(1);

            foreach (var item in TabItems.OrderBy(item => item.ModuleType.ToString()))
            {
                ImGui.PushID(item.ModuleType.ToString());

                var headerHoveredColor = ImGui.GetStyle().Colors[(int) ImGuiCol.HeaderHovered];
                var textSelectedColor = ImGui.GetStyle().Colors[(int) ImGuiCol.Header];
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerHoveredColor with {W = 0.1f});
                ImGui.PushStyleColor(ImGuiCol.Header, textSelectedColor with {W = 0.1f});

                if (ImGui.Selectable("", SelectedTabItem == item))
                {
                    SelectedTabItem = item;
                }

                ImGui.PopStyleColor(2);

                ImGui.SameLine();

                item.DrawTabItem();

                ImGui.Spacing();

                ImGui.PopID();
            }

            ImGui.EndListBox();
        }
    }
}
