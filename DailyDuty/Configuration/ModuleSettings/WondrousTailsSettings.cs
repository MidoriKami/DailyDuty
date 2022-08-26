using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.ModuleSettings;

public class WondrousTailsSettings : GenericSettings
{
    public Setting<bool> InstanceNotifications = new(false);
    public Setting<bool> EnableClickableLink = new(false);
    public Setting<bool> UnclaimedBookWarning = new(true);
    public Setting<bool> OverlayEnabled = new(true);
}