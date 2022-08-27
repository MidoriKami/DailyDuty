using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

public class MiniCactpotSettings : GenericSettings
{
    public int TicketsRemaining = 3;
    public Setting<bool> EnableClickableLink = new(false);
}