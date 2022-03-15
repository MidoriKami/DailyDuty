using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using ImGuiNET;

namespace DailyDuty.Interfaces
{
    internal interface ICompletable
    {
        public CompletionType Type { get; }
        public string HeaderText { get; }
        public GenericSettings GenericSettings { get; }

        public bool IsCompleted();
        public void DrawTask(TaskColors colors, bool showCompletedTasks)
        {
            if (IsCompleted() == false && GenericSettings.Enabled)
            {
                ImGui.TextColored(colors.IncompleteColor, HeaderText);
            }
            else if (IsCompleted() == true && GenericSettings.Enabled && showCompletedTasks)
            {
                ImGui.TextColored(colors.CompleteColor, HeaderText);
            }
        }
    }
}