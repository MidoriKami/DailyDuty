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
        private readonly List<IDisposable> windowList = new()
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
