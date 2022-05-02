using System;

namespace DailyDuty.Data.Components
{
    public class GenericSettings
    {
        public DateTime NextReset = new();
        public bool Enabled = false;
        public bool ZoneChangeReminder = false;
        public bool LoginReminder = false;
        public bool ExpandedDisplay = false;
    }
}