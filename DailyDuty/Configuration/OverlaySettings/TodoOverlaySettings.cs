using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.OverlaySettings;

internal class TodoOverlaySettings
{
    public readonly Setting<bool> ShowDailyTasks = new(true);
    public readonly Setting<bool> ShowWeeklyTasks = new(true);
    public readonly Setting<bool> HideWhenAllTasksComplete = new(true);
    public readonly Setting<bool> HideCompletedTasks = new(true);
    public readonly Setting<bool> HideUnavailableTasks = new(true);
    public readonly Setting<bool> HideWhileInDuty = new(true);
    public readonly Setting<bool> LockWindowPosition = new(false);
    public readonly Setting<float> Opacity = new(1.0f);
    public readonly Setting<WindowAnchor> AnchorCorner = new(WindowAnchor.TopLeft);
    public readonly Setting<bool> AutoResize = new(true);
    public readonly Setting<bool> Enabled = new(false);

    public readonly TaskColors TaskColors = new();
}