using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;

namespace DailyDuty.System
{
    public class SystemManager : IDisposable
    {
        private readonly List<object> dataObjects = new()
        {
            // Modules


        };

        internal readonly List<ICommand> CommandList = new();

        public SystemManager()
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
