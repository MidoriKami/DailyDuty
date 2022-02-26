using DailyDuty.Data.SettingsObjects.Windows.SubComponents;

namespace DailyDuty.Data.SettingsObjects.Windows
{
    public class TodoWindowSettings : GenericWindowSettings
    {
        public bool ShowDaily = true;
        public bool ShowWeekly = true;
        public bool HideWhenTasksComplete = false;
        public bool ShowTasksWhenComplete = false;

        public TaskColors Colors = new();
    }
}