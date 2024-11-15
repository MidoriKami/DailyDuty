using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Configuration;

namespace DailyDuty.Models;

public unsafe class SystemConfig : CharacterConfiguration {
    public bool HideDisabledModules = false;

    public static SystemConfig Load() {
        var config = Service.PluginInterface.LoadCharacterFile(PlayerState.Instance()->ContentId, "System.config.json", () => {
            var newConfig = new SystemConfig();
            newConfig.UpdateCharacterData();

            return newConfig;
        });
        
        Service.Log.Debug($"[DailyDutySystem] Logging into character: {PlayerState.Instance()->CharacterNameString}, updating System.config.json");
        config.UpdateCharacterData();
        config.Save();

        return config;
    }

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(PlayerState.Instance()->ContentId, "System.config.json", this);
}