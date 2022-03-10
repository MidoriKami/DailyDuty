using System;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ICollapsibleHeader : IDisposable
    {
        public string HeaderText { get; }

        public void Draw()
        {
            ImGui.PushID(HeaderText);

            if (ImGui.CollapsingHeader(HeaderText))
            {
                DrawContents();
            }

            ImGui.PopID();
        }

        protected void DrawContents();
    }
}