using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Interfaces;

namespace DailyDuty.Modules.Command
{
    internal class TodoWindowCommands : ICommand
    {
        private TodoWindowSettings Settings => Service.Configuration.Windows.Todo;

        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "todo"
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
