using DailyDuty.Data.Components;
using DailyDuty.Enums;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ICompletable
    {
        public CompletionType Type { get; }
        public bool IsCompleted();
        public GenericSettings GenericSettings { get; }
        public string DisplayName { get; }

        public void DrawTask(TaskColors colors, bool showCompletedTasks)
        {
            if (IsCompleted() == false && GenericSettings.Enabled)
            {
                ImGui.TextColored(colors.IncompleteColor, DisplayName);
            }
            else if (IsCompleted() == true && GenericSettings.Enabled && showCompletedTasks)
            {
                ImGui.TextColored(colors.CompleteColor, DisplayName);
            }
        }
    }
}