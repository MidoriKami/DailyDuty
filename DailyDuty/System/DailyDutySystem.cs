using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Features;

namespace DailyDuty.System
{
    public class DailyDutySystem : IDisposable
    {
        private readonly List<object> dataObjects = new()
        {
            // Modules
            new WondrousTailsDutyFinderOverlay(),

        };

        internal readonly List<ICommand> CommandList = new();

        public DailyDutySystem()
        {
            CommandList.AddRange(dataObjects.OfType<ICommand>());
        }

        public void ExecuteCommand(string command, string arguments)
        {
            foreach (var eachCommand in CommandList)
            {
                eachCommand.ProcessCommand(command, arguments);
            }
        }

        public void Dispose()
        {
            foreach (var module in dataObjects.OfType<IDisposable>())
            {
                module.Dispose();
            }
        }
    }
}
