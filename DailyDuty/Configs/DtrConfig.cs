using DailyDuty.Classes;
using Dalamud.Bindings.ImGui;
using KamiLib.Configuration;

namespace DailyDuty.Configs;

public class DtrConfig {

	public bool SoloDaily;
	public bool SoloWeekly;
	public bool Combo = true;
	public bool HideSeconds;

	public DtrMode CurrentMode = DtrMode.Daily;

	public static DtrConfig Load() 
		=> Service.PluginInterface.LoadCharacterFile<DtrConfig>(Service.ClientState.LocalContentId, "DTR.config.json");

	public void Save()
		=> Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "DTR.config.json", this);

	public void DrawConfig() {
		var configChanged = false;

		configChanged |= ImGui.Checkbox("Enable Combo Timer", ref Combo);
		configChanged |= ImGui.Checkbox("Enable Individual Daily Timer", ref SoloDaily);
		configChanged |= ImGui.Checkbox("Enable Individual Weekly Timer", ref SoloWeekly);

		ImGui.Spacing();
		
		configChanged |= ImGui.Checkbox("Hide Seconds", ref HideSeconds);

		if (configChanged) {
			Save();
		}
	}
}