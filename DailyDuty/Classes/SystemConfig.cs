using DailyDuty.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DailyDuty.Classes;

public class SystemConfig {
    public int Version = 3;
    public bool EnableSceneEventLogging;

    public HashSet<string> EnabledModules = [];

    public static async Task<SystemConfig> Load() {
        Services.PluginLog.Debug("Loading system.config.json");
        return await Config.LoadCharacterConfig<SystemConfig>("system.config.json");
    }

    public async Task Save() {
        Services.PluginLog.Debug("Saving system.config.json");
        await Config.SaveCharacterConfig(this, "system.config.json");
    }
}
