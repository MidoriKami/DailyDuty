using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiLib.Configuration;

namespace DailyDuty.Models;

public class TimersConfig {

	public bool Enabled = false;
	
	public bool HideInDuties = true;
	public bool HideInQuestEvents = true;
	
	public static TimersConfig Load() 
		=> Service.PluginInterface.LoadCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", () => new TimersConfig());

	public void Save()
		=> Service.PluginInterface.SaveCharacterFile(Service.ClientState.LocalContentId, "Timers.config.json", this);
}

public class TimerConfig {
	public bool TimerEnabled = false;
	public Vector2 Position = new(400.0f, 400.0f);
	public Vector2 Size = new(400.0f, 32.0f);
	public Vector4 BarColor = KnownColor.Aqua.Vector();
	public Vector4 BarBackgroundColor = KnownColor.Black.Vector();
	public bool HideWhenComplete = true;
	public bool HideName = false;
	public bool HideTime = false;
	public bool HideSeconds = false;
	public bool UseCustomLabel = false;
	public string CustomLabel = string.Empty;
}
