using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Notice;
using DailyDuty.Windows.Timers;

namespace DailyDuty.Modules.Command
{
    internal class TimersWindowCommands : ICommand
    {
        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "timers",
            "timer"
        };

        void ICommand.Execute(string primaryCommand, string? secondaryCommand)
        {
            Service.Configuration.Windows.Timers.Open = !Service.Configuration.Windows.Timers.Open;
        }
    }
}
