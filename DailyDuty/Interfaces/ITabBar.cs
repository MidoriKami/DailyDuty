using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ITabBar
    {
        string TabBarName { get; }

        List<ITab> Tabs { get; }

        void Draw()
        {
            if (ImGui.BeginTabBar(TabBarName, ImGuiTabBarFlags.Reorderable))
            {
                foreach (var tab in Tabs)
                {
                    if (!ImGui.BeginTabItem(tab.TabName))
                        continue;
                    
                    if (ImGui.BeginChild(TabBarName + "TabBarChild", new Vector2(0,0), true))
                    {
                        tab.Draw();
                        ImGui.EndChild();
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
    }
}
