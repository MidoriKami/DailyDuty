using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyDuty.Interfaces
{
    internal interface IZoneChangeLogic
    {
        public void HandleZoneChange(object? sender, ushort e);
    }
}
