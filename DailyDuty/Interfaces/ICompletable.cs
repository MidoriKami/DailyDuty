using System;
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
        public Action? ExpandedDisplay { get; }

        public void DrawTask(TaskColors colors, bool showCompletedTasks)
        {
            if (GenericSettings.ExpandedDisplay && GenericSettings.Enabled)
            {
                ExpandedDisplay?.Invoke();
            }
            else
            {
                if (!IsCompleted() && GenericSettings.Enabled)
                {
                    ImGui.TextColored(colors.IncompleteColor, DisplayName);
                }
                else if (IsCompleted() && GenericSettings.Enabled && showCompletedTasks)
                {
                    ImGui.TextColored(colors.CompleteColor, DisplayName);
                }
            }
        }
    }
}