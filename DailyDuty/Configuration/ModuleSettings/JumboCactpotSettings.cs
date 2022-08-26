using System.Collections.Generic;
using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

public class JumboCactpotSettings : GenericSettings
{
    public List<int> Tickets = new();
    public Setting<bool> EnableClickableLink = new(false);
}