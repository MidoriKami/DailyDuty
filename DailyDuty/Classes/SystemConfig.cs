using System.Collections.Generic;

namespace DailyDuty.Classes;

public class SystemConfig {
    public int Version = 3;
    public string CharacterName = "Unknown Name";
    public string CharacterWorld = "Unknown World";
    public ulong ContentId;
    public string? LodestoneId;
    public bool EnableChatLinks = true;

    public HashSet<string> EnabledModules = [];
    
    public static SystemConfig Load()
        => Utilities.Config.LoadCharacterConfig<SystemConfig>("system.config.json");

    public void Save()
        => Utilities.Config.SaveCharacterConfig(this, "system.config.json");
}
