using DailyDuty.Configuration.Components;

namespace DailyDuty.Configuration.OverlaySettings;

internal class TimersOverlaySettings
{
    public Setting<bool> Enabled = new(false);
    public Setting<bool> HideWhileInDuty = new(true);
    public Setting<bool> LockWindowPosition = new(false);
    public Setting<bool> AutoResize = new(true);
    public Setting<bool> HideCompleted = new(false);
    public Setting<float> Opacity = new(1.0f);
}