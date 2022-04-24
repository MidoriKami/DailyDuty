using DailyDuty.Enums;

namespace DailyDuty.Data.Components
{
    public class TodoWindowSettings
    {
        public bool ShowDaily = true;
        public bool ShowWeekly = true;
        public bool HideWhenTasksComplete = false;
        public bool ShowTasksWhenComplete = false;
        public WindowAnchor Anchor = WindowAnchor.TopLeft;
        public TodoWindowStyle Style = TodoWindowStyle.AutoResize;

        public TaskColors Colors = new();
    }
}