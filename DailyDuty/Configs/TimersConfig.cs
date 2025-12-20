using KamiLib.Configuration;

namespace DailyDuty.Configs;

public class TimersConfig {
    public bool Enabled = false;
	
    public bool HideInDuties = true;
    public bool HideInQuestEvents = true;
    public bool HideTimerSeconds = false;

    public bool EnableDailyTimer = true;
    public bool EnableWeeklyTimer = true;
    
    public static TimersConfig Load() 
        => Service.PluginInterface.LoadCharacterFile<TimersConfig>(Service.PlayerState.ContentId, "Timers.config.json");

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.PlayerState.ContentId, "Timers.config.json", this);
}
