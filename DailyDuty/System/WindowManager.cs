using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Notice;
using DailyDuty.Windows.Settings;
using DailyDuty.Windows.Timers;
using DailyDuty.Windows.Todo;

namespace DailyDuty.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IWindow> windowList = new()
        {
            new SettingsWindow(),
            new TodoWindow(),
            new TimersWindow(),
            new NoticeWindow(),

            // Debug Windows
            //new HuntMarkHelper()
        };

        public void Dispose()
        {
            foreach (var window in windowList)
            {
                window.Dispose();
            }
        }

        public T? GetWindowOfType<T>(WindowName name)
        {
            var settingsWindow = windowList
                .Where(w => w.WindowName == name)
                .OfType<T>()
                .FirstOrDefault();

            return settingsWindow;
        }
    }
}
