using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Windows.DailyDutyWindow;
using DailyDuty.Windows.HuntHelperWindow;
using DailyDuty.Windows.LogBrowserWindow;
using DailyDuty.Windows.TimersWindow;
using DailyDuty.Windows.TodoWindow;

namespace DailyDuty.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IDisposable> windowList = new()
        {
            new DailyDutyWindow(),
            new LogBrowserWindow(),
            new TodoWindow(),
            new TimersWindow(),
            new HuntHelperWindow()
        };

        public void Dispose()
        {
            foreach (var window in windowList)
            {
                window.Dispose();
            }
        }

        public T? GetWindowOfType<T>()
        {
            return windowList.OfType<T>().FirstOrDefault();
        }

        public void ExecuteCommand(string command, string arguments)
        {
            foreach (var eachCommand in windowList.OfType<ICommand>())
            {
                eachCommand.ProcessCommand(command, arguments);
            }
        }
    }
}
