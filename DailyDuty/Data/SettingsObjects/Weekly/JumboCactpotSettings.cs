using System.Collections.Generic;

namespace DailyDuty.Data.SettingsObjects.Weekly
{
    public class JumboCactpotSettings : GenericSettings
    {
        public List<int> Tickets = new();
        public uint PlayerRegion = 0;
    }
}