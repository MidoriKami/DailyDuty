using System.Collections.Generic;
using DailyDuty.Enums;

namespace DailyDuty.Data.Components
{
    internal class WondrousTailsTask
    {
        public ButtonState TaskState { get; set; }
        public List<uint> DutyList { get; init; } = new();
    }
}
