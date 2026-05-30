using System.Collections.Generic;
using System.Threading.Tasks;
using DailyDuty.Utilities;

namespace DailyDuty.Classes;

public class SystemConfig {
    public int Version = 3;
    public string CharacterName = "Unknown Name";
    public string CharacterWorld = "Unknown World";
    public ulong ContentId;
    public string? LodestoneId;

    public HashSet<string> EnabledModules = [];

    public static async Task<SystemConfig> Load()
        => await Config.LoadCharacterConfig<SystemConfig>("system.config.json");

    public async Task Save()
        => await Config.SaveCharacterConfig(this, "system.config.json");
}
