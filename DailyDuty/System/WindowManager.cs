using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.DailyDutyWindow;

namespace DailyDuty.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IWindow> windowList = new()
        {
            new DailyDutyWindow(),
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
