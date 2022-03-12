using DailyDuty.Data.SettingsObjects.Notice;
using DailyDuty.Data.SettingsObjects.Windows;

namespace DailyDuty.Data.SettingsObjects
{
    public class WindowsSettings
    {
        public TodoWindowSettings Todo = new();
        public SettingsWindowSettings Settings = new();
        public TimersWindowSettings Timers = new();
        public NoticeSettings Notice = new();
    }
}
