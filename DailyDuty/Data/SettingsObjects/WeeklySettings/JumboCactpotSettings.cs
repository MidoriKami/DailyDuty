using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.ModuleData.JumboCactpot;

namespace DailyDuty.Data.SettingsObjects.WeeklySettings;

public class JumboCactpotSettings : GenericSettings
{
    public List<TicketData> CollectedTickets = new();
    public uint PlayerRegion = 0;
}