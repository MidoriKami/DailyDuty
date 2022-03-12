using System.Collections.Generic;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.Notice;

namespace DailyDuty.Modules.Command
{
    internal class NoticeWindowCommands : ICommand
    {
        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "help",
            "info",
            "debug"
        };

        void ICommand.Execute(string primaryCommand, string? secondaryCommand)
        {
            var window = Service.WindowManager.GetWindowOfType<NoticeWindow>(WindowName.Notice);

            window?.Reset();
        }
    }
}
