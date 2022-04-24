using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Enums;

namespace DailyDuty.Data.Components
{
    internal class WondrousTailsTask
    {
        public ButtonState TaskState { get; set; }
        public List<uint> DutyList { get; init; } = new List<uint>();
    }
}
