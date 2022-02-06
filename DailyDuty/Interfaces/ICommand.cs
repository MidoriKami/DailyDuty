using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Interfaces
{
    internal interface ICommand
    {
        public void ProcessCommand(string command);
    }
}
