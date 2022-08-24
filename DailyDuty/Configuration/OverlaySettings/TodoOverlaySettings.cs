using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;

namespace DailyDuty.Configuration.OverlaySettings;

internal class TodoOverlaySettings
{
    public Setting<bool> ShowDailyTasks = new(true);
    public Setting<bool> ShowWeeklyTasks = new(true);
    public Setting<bool> HideWhenAllTasksComplete = new(true);
    public Setting<bool> HideCompletedTasks = new(true);
    public Setting<bool> HideUnavailableTasks = new(true);
    public Setting<bool> HideWhileInDuty = new(true);
    public Setting<bool> LockWindowPosition = new(false);
    public Setting<float> Opacity = new(1.0f);
    public Setting<WindowAnchor> AnchorCorner = new(WindowAnchor.TopLeft);
    public Setting<bool> AutoResize = new(true);
    public Setting<bool> Enabled = new(false);

    public readonly TaskColors TaskColors = new();
}