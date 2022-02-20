using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Settings;
using DailyDuty.Windows.Timers;
using DailyDuty.Windows.Todo;
using Dalamud.Interface.Windowing;

namespace DailyDuty.System
{
    public class WindowManager : IDisposable
    {
        private readonly List<IWindow> windowList = new()
        {
            new SettingsWindow(),
            new TodoWindow(),
            new TimersWindow()
        };

        public WindowManager()
        {

        }

        public void Dispose()
        {
            foreach (var window in windowList)
            {
                window.Dispose();
            }
        }

        public void ToggleSettingsWindow()
        {
            var settingsWindow = windowList
                .Where(w => w.WindowName == WindowName.Settings)
                .OfType<Window>()
                .First();

            settingsWindow.IsOpen = !settingsWindow.IsOpen;
        }
    }
}
