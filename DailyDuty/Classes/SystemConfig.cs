using System.Collections.Generic;
using DailyDuty.Utilities;

namespace DailyDuty.Classes;

public class SystemConfig {
    public int Version = 3;
    public string CharacterName = "Unknown Name";
    public string CharacterWorld = "Unknown World";
    public ulong ContentId;
    public string? LodestoneId;

    public HashSet<string> EnabledModules = [];
    
    public static SystemConfig Load()
        => Config.LoadCharacterConfig<SystemConfig>("system.config.json");

    public void Save()
        => Config.SaveCharacterConfig(this, "system.config.json");
}
