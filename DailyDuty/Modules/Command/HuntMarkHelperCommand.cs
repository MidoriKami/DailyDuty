using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Windows.HuntMark;
using DailyDuty.Windows.Notice;

namespace DailyDuty.Modules.Command
{
    internal class HuntMarkHelperCommand : ICommand
    {
        List<string> ICommand.ModuleCommands { get; } = new()
        {
            "hunthelper"
        };

        void ICommand.Execute(string primaryCommand, string? secondaryCommand)
        {
            var window = Service.WindowManager.GetWindowOfType<HuntMarkHelper>(WindowName.HuntHelper);

            window?.Toggle();
        }
    }
}
