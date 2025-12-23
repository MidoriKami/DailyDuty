using DailyDuty.Classes;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlayConfig : ConfigBase {
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;
    public bool HideTimerSeconds = false;

    public bool EnableDailyTimer = true;
    public bool EnableWeeklyTimer = true;
}
