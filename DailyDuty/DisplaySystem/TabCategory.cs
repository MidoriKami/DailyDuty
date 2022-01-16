using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.DisplaySystem.DisplayModules;
using ImGuiNET;

namespace DailyDuty.DisplaySystem
{
    internal abstract class TabCategory : IDisposable
    {
        protected List<DisplayModule> Modules = new();
        protected uint FrameID;
        public string CategoryName { get; protected set; } = "Unset CategoryName";
        public string TabName { get; protected set; } = "Unset TabName";

        public void Draw()
        {
            ImGui.Text(CategoryName);
            ImGui.Separator();
            ImGui.Spacing();

            ImGui.BeginChildFrame(FrameID, new Vector2(490, 365), ImGuiWindowFlags.NoBackground);

            foreach (var module in Modules)
            {
                module.Draw();
            }

            ImGui.EndChildFrame();

            ImGui.Spacing();
        }

        public void Dispose()
        {
            foreach (var module in Modules)
            {
                module.Dispose();
            }
        }
    }
}
