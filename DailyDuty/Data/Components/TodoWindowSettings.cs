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
        public bool Enabled = false;
        public bool HideInDuty = true;
        public float Opacity = 1.0f;
        public bool ClickThrough = false;

        public TaskColors Colors = new();
    }
}