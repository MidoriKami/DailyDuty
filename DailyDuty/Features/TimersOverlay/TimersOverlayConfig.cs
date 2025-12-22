using DailyDuty.Classes;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlayConfig : ModuleConfig<TimersOverlayConfig> {
    protected override string FileName => "Timers";
    
    public bool Enabled = false;
	
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;
    public bool HideTimerSeconds = false;

    public bool EnableDailyTimer = true;
    public bool EnableWeeklyTimer = true;
}
