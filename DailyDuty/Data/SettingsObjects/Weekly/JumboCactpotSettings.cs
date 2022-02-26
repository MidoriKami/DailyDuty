using System.Collections.Generic;
using DailyDuty.Data.ModuleData.JumboCactpot;

namespace DailyDuty.Data.SettingsObjects.Weekly
{
    public class JumboCactpotSettings : GenericSettings
    {
        public List<TicketData> CollectedTickets = new();
        public uint PlayerRegion = 0;
    }
}