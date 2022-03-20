using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Interfaces;

namespace DailyDuty.Modules.Command
{
    internal class TodoWindowCommands : ICommand
    {
        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "todo"
        };

        void ICommand.Execute(string primaryCommand, string? secondaryCommand)
        {
            Service.Configuration.Windows.Todo.Open = !Service.Configuration.Windows.Todo.Open;
        }
    }
}
