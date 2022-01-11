using System;
using ImGuiNET;

namespace DailyDuty.Reminders
{
    internal abstract class TabCategory : IDisposable
    {
        public string CategoryName { get; protected set; } = "Unset CategoryName";
        public string TabName { get; protected set; } = "Unset TabName";

        protected abstract void DrawContents();

        public void Draw()
        {
            ImGui.Text(CategoryName);
            ImGui.Separator();
            ImGui.Spacing();

            DrawContents();

            ImGui.Spacing();
        }

        public abstract void Dispose();
    }
}
