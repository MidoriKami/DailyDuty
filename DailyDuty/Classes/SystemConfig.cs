using DailyDuty.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace DailyDuty.Classes;

public class SystemConfig {
    public int Version = 3;
    public bool EnableSceneEventLogging;
    public bool IsDebugMode = false;

    public HashSet<string> EnabledModules = [];

    public static async Task<SystemConfig> Load() {
        IPluginLog.Get().Debug("Loading system.config.json");
        return await Config.LoadCharacterConfig<SystemConfig>("system.config.json");
    }

    public async Task Save() {
        IPluginLog.Get().Debug("Saving system.config.json");
        await Config.SaveCharacterConfig(this, "system.config.json");
    }
}
