using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;
using DailyDuty.Windows.DailyDutyWindow;
using Dalamud.Interface.Windowing;

namespace DailyDuty.System
{
    public class DailyDutySystem : IDisposable
    {
        // Windows
        private readonly List<Window> windows = new()
        {
            new DailyDutyWindow(),
        };

        // Modules
        private readonly List<IModule> modules = new()
        {

        };

        private readonly List<ICommand> commandList = new();

        public DailyDutySystem()
        {
            commandList.AddRange(windows.OfType<ICommand>());
        }

        public void ExecuteCommand(string command, string arguments)
        {
            foreach (var eachCommand in commandList)
            {
                eachCommand.ProcessCommand(command, arguments);
            }
        }

        public void Dispose()
        {
        }
    }
}
