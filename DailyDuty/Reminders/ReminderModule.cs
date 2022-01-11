using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace DailyDuty.Reminders
{
    internal abstract class ReminderModule : IDisposable
    {
        public string CategoryString = "CategoryString Not Set";

        protected abstract void DrawContents();

        public void Draw()
        {
            ImGui.Text(CategoryString);
            ImGui.Separator();
            ImGui.Spacing();

            DrawContents();

            ImGui.Spacing();
        }

        public abstract void Dispose();
    }
}
