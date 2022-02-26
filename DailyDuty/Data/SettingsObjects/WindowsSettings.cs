using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects.Addons;
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
