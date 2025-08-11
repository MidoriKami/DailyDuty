using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Configuration;

namespace DailyDuty.Configs;

public unsafe class SystemConfig : CharacterConfiguration {
    public bool HideDisabledModules = false;

    public static SystemConfig Load() {
        var config = Service.PluginInterface.LoadCharacterFile<SystemConfig>(PlayerState.Instance()->ContentId, "System.config.json");

        Service.Log.Debug($"[DailyDutySystem] Logging into character: {PlayerState.Instance()->CharacterNameString}, updating System.config.json");
        config.UpdateCharacterData();
        config.Save();

        return config;
    }

    public void Save()
        => Service.PluginInterface.SaveCharacterFile(PlayerState.Instance()->ContentId, "System.config.json", this);
}