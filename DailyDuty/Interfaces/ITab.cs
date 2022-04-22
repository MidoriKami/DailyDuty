using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ITab
    {
        IConfigurable? SelectedTabItem { get; set; }
        List<IConfigurable> TabItems { get; set; }
        string TabName { get; }

        string Description { get; }

        public void DrawTabContents()
        {
            ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);

            if (ImGui.BeginListBox("", new Vector2(-1, -1)))
            {

                foreach (var item in TabItems)
                {

                    ImGui.PushID(item.ConfigurationPaneLabel);

                    if (ImGui.Selectable("", SelectedTabItem == item))
                    {
                        SelectedTabItem = item;
                    }

                    ImGui.SameLine();

                    item.DrawTabItem();

                    ImGui.Spacing();

                    ImGui.PopID();
                }

                ImGui.EndListBox();
            }

            ImGui.PopStyleColor();
        }
    }
}
