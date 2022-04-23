using System;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using ImGuiNET;

namespace DailyDuty.Data.Components
{
    public class LogMessage
    {
        public ModuleType ModuleType { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; } = string.Empty;

        public void Draw()
        {
            ImGui.TextColored(Colors.Grey, $"[{Time.ToLocalTime()}]:");
            ImGui.SameLine();
            ImGui.Text(Message);
        }
    }
}
