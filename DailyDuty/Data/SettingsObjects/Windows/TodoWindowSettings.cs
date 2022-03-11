using DailyDuty.Data.SettingsObjects.Windows.SubComponents;
using DailyDuty.Data.Enums;

namespace DailyDuty.Data.SettingsObjects.Windows
{
    public class TodoWindowSettings : GenericWindowSettings
    {
        public bool ShowDaily = true;
        public bool ShowWeekly = true;
        public bool HideWhenTasksComplete = false;
        public bool ShowTasksWhenComplete = false;
        public WindowAnchor Anchor = WindowAnchor.TopLeft;

        public TaskColors Colors = new();
    }
}