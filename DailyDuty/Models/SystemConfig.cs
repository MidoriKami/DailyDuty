using KamiLib.Configuration;

namespace DailyDuty.Models;

public class SystemConfig : CharacterConfiguration {
    public bool HideDisabledModules = false;

    public static SystemConfig Load() {
        var config = Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "System.config.json", () => {
            var newConfig = new SystemConfig();
            newConfig.UpdateCharacterData(Service.ClientState);

            return newConfig;
        });
        
        Service.Log.Debug($"[DailyDutySystem] Logging into character: {Service.ClientState.LocalPlayer?.Name}, updating System.config.json");
        config.UpdateCharacterData(Service.ClientState);
        config.Save();

        return config;
    }

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "System.config.json", this);
}