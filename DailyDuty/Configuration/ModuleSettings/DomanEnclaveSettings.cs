using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

internal class DomanEnclaveSettings : GenericSettings
{
    public int DonatedThisWeek = 0;
    public int WeeklyAllowance = 0;

    public Setting<bool> EnableClickableLink = new(true);
}