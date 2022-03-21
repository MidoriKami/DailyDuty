using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Notice;
using DailyDuty.Windows.Timers;

namespace DailyDuty.Modules.Command
{
    internal class TimersWindowCommands : ICommand
    {
        private TimersWindowSettings Settings => Service.Configuration.Windows.Timers;

        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "timers",
            "timer"
        };

        void ICommand.Execute(string primaryCommand, string? secondaryCommand)
        {
            var lastState = Settings.Open;

            Settings.Open = secondaryCommand switch
            {
                null => !Settings.Open,
                "on" or "yes" or "show" or "enable" => true,
                "off" or "no" or "hide" or "disable" => false,
                _ => Settings.Open
            };

            if (lastState != Settings.Open)
            {
                Service.Configuration.Save();
            }
        }
    }
}
