using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.OverlaySettings;

internal class TimersOverlaySettings
{
    public readonly Setting<bool> Enabled = new(false);
    public readonly Setting<bool> HideWhileInDuty = new(true);
    public readonly Setting<bool> LockWindowPosition = new(false);
    public readonly Setting<bool> AutoResize = new(false);
    public readonly Setting<float> Opacity = new(1.0f);
}